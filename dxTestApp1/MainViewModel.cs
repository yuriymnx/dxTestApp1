using DevExpress.Mvvm;
using DevExpress.Xpf.RichEdit;
using System.Windows.Input;

namespace dxTestApp1;

class MainViewModel
{
    public ICommand SaveCommand => new DelegateCommand<RichEditControl>(Save);

    private void Save(RichEditControl editor)
    {
        MessageBox.Show("Saved");
    }
}