using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace HeightLightCode.VMBase
{
    public class YINotify : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void Notify<R>(Expression<Func<INotifyPropertyChanged,R>> expre)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(((MemberExpression)expre.Body).Member.Name));
        }
    }
}
