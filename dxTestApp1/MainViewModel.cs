using DevExpress.Mvvm;
using DevExpress.Xpf.RichEdit;
using DevExpress.XtraRichEdit;
using System.IO;
using System.Windows.Input;

namespace dxTestApp1;

class MainViewModel
{
    public Stream DocumentSource { get; set; }

    public MainViewModel()
    {
        using var fs =  File.OpenRead("C:\\Users\\Yury\\Desktop\\file-sample_100kB.doc");
        var ms = new MemoryStream();
        fs.CopyTo(ms);
        DocumentSource = ms;
    }

    public ICommand SaveCommand => new DelegateCommand<RichEditControl>(Save);

    private void Save(RichEditControl editor)
    {
        using var ms = new MemoryStream();
        editor.Document.SaveDocument(ms, DocumentFormat.Doc);
        using var fw = File.Create("C:\\Users\\Yury\\Desktop\\1.doc");
        ms.Seek(0, SeekOrigin.Begin);
        ms.CopyTo(fw);
    }
}