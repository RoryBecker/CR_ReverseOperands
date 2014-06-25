using System.ComponentModel.Composition;
using DevExpress.CodeRush.Common;

namespace CR_ReverseOperands
{
    [Export(typeof(IVsixPluginExtension))]
    public class CR_ReverseOperandsExtension : IVsixPluginExtension { }
}