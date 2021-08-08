using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Media;

namespace HeightLightCode.Models
{
    /// <summary>
    /// 字号枚举
    /// </summary>
    public enum FontSize
    {
        初号 = 56,
        小初 = 48,
        一号 = 34,
        小一 = 32,
        二号 = 29,
        小二 = 24,
        三号 = 21,
        小三 = 20,
        四号 = 18,
        小四 = 16,
        五号 = 14,
        小五 = 12,
        六号 = 10,
        小六 = 8,
        七号 = 7,
        八号 = 6,
        
    }

    public enum LanguageEnum
    {
        AppleScript,
        AS3,
        Bash,
        ColdFusion,
        Cpp,
        CSharp,
        Css,
        Delphi,
        Diff,
        Erlang,
        Groovy,
        Java,
        JavaFX,
        JScript,
        Perl,
        Php,
        Plain,
        PowerShell,
        Python,
        Ruby,
        Sass,
        Scala,
        Sql,
        Vb,
        Xml,
    }


    public class SettingModel
    {
        /// <summary>
        /// 字体名称
        /// </summary>
        public string FontFamily { get; set; }

        /// <summary>
        /// 字体集合
        /// </summary>
        [JsonIgnore]
        public ObservableCollection<string> FontFamilyCollection { get; set; }

        /// <summary>
        /// 字号大小
        /// </summary>
        public FontSize FontSize { get; set; }

        /// <summary>
        /// 字号集合
        /// </summary>
        [JsonIgnore]
        public ObservableCollection<FontSize> FontSizeCollection { get; set; }

        /// <summary>
        /// 代码样式
        /// </summary>
        public string CodeStyle { get; set; }

        /// <summary>
        /// 代码样式集合
        /// </summary>
        [JsonIgnore]
        public ObservableCollection<string> CodeStyleCollection { get; set; }

        /// <summary>
        /// 编程语言
        /// </summary>
        public LanguageEnum CodeLanguage { get; set; }

        /// <summary>
        /// 编程语言集合
        /// </summary>
        [JsonIgnore]
        public ObservableCollection<LanguageEnum> CodeLanguageCollection { get; set; }

        /// <summary>
        /// 行号
        /// </summary>
        public bool GutterStatus { get; set; }

        public SettingModel()
        {
            FontFamilyCollection = GetFontFamilyNames();
            FontFamily = FontFamilyCollection.FirstOrDefault();
            FontSizeCollection = new ObservableCollection<FontSize>(Enum.GetValues(typeof(FontSize)).Cast<FontSize>());
            FontSize = FontSizeCollection.FirstOrDefault();
            CodeStyleCollection = GetStyles();
            CodeStyle = CodeStyleCollection.FirstOrDefault();
            CodeLanguageCollection = new ObservableCollection<LanguageEnum>(Enum.GetValues(typeof(LanguageEnum)).Cast<LanguageEnum>());
            CodeLanguage = CodeLanguageCollection.FirstOrDefault();
            GutterStatus = false;
        }

        private ObservableCollection<string> GetStyles()
        {
            string styleDirectoryPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\Datas\SyntaxHighLighter\styles");
            //拿到所有的文件
            List<string> cssFileList = Directory.GetFiles(styleDirectoryPath).ToList();
            ObservableCollection<string> stylesColl = new ObservableCollection<string>();
            foreach (var file in cssFileList)
            {
                if (Path.GetExtension(file) == ".css"&&file.Contains("shTheme"))
                {
                    stylesColl .Add(Path.GetFileNameWithoutExtension(file).Replace("shTheme",""));
                }
            }
            return stylesColl;
        }

        private ObservableCollection<string> GetFontFamilyNames()
        {
            List<string> fontFamilyNameColl = new List<string>();
            foreach (var fontFamily in Fonts.SystemFontFamilies)
            {
                LanguageSpecificStringDictionary lsd = fontFamily.FamilyNames;
                if (lsd.ContainsKey(XmlLanguage.GetLanguage("zh-cn")))
                {
                    string fontname = null;
                    if (lsd.TryGetValue(XmlLanguage.GetLanguage("zh-cn"), out fontname))
                    {
                        fontFamilyNameColl.Add(fontname);
                    }
                }
                else
                {
                    string fontname = null;
                    if (lsd.TryGetValue(XmlLanguage.GetLanguage("en-us"), out fontname))
                    {
                        fontFamilyNameColl.Add(fontname);
                    }
                }
            }
            return new ObservableCollection<string>(fontFamilyNameColl.OrderBy(x=>x));
        }

    }
}
