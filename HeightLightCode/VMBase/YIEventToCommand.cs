using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace HeightLightCode.VMBase
{
    public class YIEventToCommand : TriggerAction<DependencyObject>
    {
        private string commandName;

        public ICommand Command
        {
            get { return (ICommand)base.GetValue(CommandProperty); }
            set { base.SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// 获取或设置此操作应调用的命令，只是依赖属性
        /// </summary>
        /// /// <value>要执行的命令。</value>
        /// <remarks>如果设置了此属性和 CommandName 属性，则此属性将优先于后者。</remarks>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(YIEventToCommand), null);


        public object CommandParameter
        {
            get { return (object)base.GetValue(CommandParameterProperty); }
            set { base.SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        /// 获得或设置参数，这是依赖属性
        /// <value>命令参数</value>
        /// <remarks>这是传递给 ICommand.CanExecute 和 ICommand.Execute 的值</remarks>
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(YIEventToCommand), new PropertyMetadata(null, (DependencyObject s, DependencyPropertyChangedEventArgs e) =>
            {
                YIEventToCommand sender = s as YIEventToCommand;
                if (sender == null)
                {
                    return;
                }
                if (sender.AssociatedObject == null)
                {
                    return;
                }
            }));


        public bool PassToEventArgs
        {
            get { return (bool)GetValue(PassToEventArgsProperty); }
            set { SetValue(PassToEventArgsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PassToEventArgs.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PassToEventArgsProperty =
            DependencyProperty.Register("PassToEventArgs", typeof(bool), typeof(YIEventToCommand),new PropertyMetadata(false));


        public string CommandName
        {
            get
            {
                base.ReadPreamble();
                return this.commandName;
            }
            set
            {
                if(this.commandName!=value)
                {
                    base.WritePreamble();
                    this.commandName = value;
                    base.WritePostscript();
                }

            }
        }

        /// <summary>
        /// 调用操作
        /// </summary>
        /// <param name="parameter"></param>
        protected override void Invoke(object parameter)
        {
            if (base.AssociatedObject == null)
                return;
            ICommand command = this.ResolveCommand();

            /*
             * ★★★★★★★★★★★★★★★★★★★★★★★★
             * 注意这里添加了事件触发源和事件参数
             * ★★★★★★★★★★★★★★★★★★★★★★★★
             */
            YICommandParameter exParameter = new YICommandParameter
            {
                Parameter = CommandParameter
            };
            if (PassToEventArgs)
            {
                exParameter.Sender = base.AssociatedObject;
                exParameter.EventArgs = parameter as EventArgs;
            }

            if (command != null && command.CanExecute(exParameter))
            {
                /*
                 * ★★★★★★★★★★★★★★★★★★★★★★★★
                 * 注意将扩展的参数传递到Execute方法中
                 * ★★★★★★★★★★★★★★★★★★★★★★★★
                 */
                command.Execute(exParameter);
            }

        }
        private ICommand ResolveCommand()
        {
            //没有直接绑定命令
            if (this.Command != null)
                return this.Command;
            if (base.AssociatedObject == null)
                return null;
            //绑定事件名称
            ICommand result = null;

            //拿到触发的命令
            Type type = base.AssociatedObject.GetType();
            //拿到所有的属性
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            //根据命令名称拿到对应的命令对象
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo propertyInfo = properties[i];
                if (typeof(ICommand).IsAssignableFrom(propertyInfo.PropertyType) && string.Equals(propertyInfo.Name, this.CommandName, StringComparison.Ordinal))
                {
                    result = (ICommand)propertyInfo.GetValue(base.AssociatedObject, null);
                    break;
                }
            }
            return result;
        }

    }
}
