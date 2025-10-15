using Ion;
using Ion.Analysis;
using Ion.Core;
using Ion.Input;
using Ion.Storage;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows.Input;

namespace Ion.Tools.Rename;

/// <remarks>https://stackoverflow.com/questions/923771/quickest-way-to-convert-a-base-10-number-to-any-base-in-net</remarks>
[NotComplete]
public record class AppViewModel : AppToolViewModel
{
    private bool isDoing = false;

    /// <see cref="Region.Property"/>

    public ObservableCollection<FilePreview> Files { get => Get<ObservableCollection<FilePreview>>(); set => Set(value); }

    public Int32 FileExtension { get => Get(2); set => Set(value); }

    public String FileNameIncrement { get => Get("1"); set => Set(value); }

    public Int32 FileNameIncrementBase { get => Get(1); set => Set(value); }

    [NotImplemented]
    public Int32 FileNameIncrementBy { get => Get(0); set => Set(value); }

    public String FileNameIncrementStart { get => Get("1"); set => Set(value); }

    public Int32 FileSort { get => Get(1); set => Set(value); }

    public Int32 FileSortDirection { get => Get(0); set => Set(value); }
    
    public String FolderPath { get => Get(""); set => Set(value); }

    public Result Result { get => Get(default(Result)); set => Set(value); }

    public override string Title => nameof(Rename);

    /// <see cref="Region.Constructor"/>

    public AppViewModel() : base()
    {
        Files = [];
    }

    /// <see cref="Region.Method"/>

    private static string getBase(int value, char[] baseChars)
    {
        string result = string.Empty;
        int targetBase = baseChars.Length;

        do
        {
            result = baseChars[value % targetBase] + result;
            value = value / targetBase;
        }
        while (value > 0);

        return result;
    }

    ///

    private static Random random = new Random();

    [NotOptimized]
    private static string getRandomString(int length)
    {
        var characters = "abcdefghijklmnopqrstuvwxyz0123456789";

        var result = new StringBuilder();

        for (int i = 0; i < length; i++)
        {
            result.Append(characters[random.Next(characters.Length)]);
        }
        return result.ToString();
    }

    ///

    private IEnumerable<FileInfo> getFiles(DirectoryInfo folder)
    {
        var searchPattern = "*.*";
        var searchQuery = folder.GetFiles(searchPattern);

        switch (FileSortDirection)
        {
            /// Ascending
            default:
            case 0:
                switch (FileSort)
                {
                    default:
                    case 0:
                        return searchQuery.OrderBy(y => y.LastAccessTime);
                    case 1:
                        return searchQuery.OrderBy(y => y.CreationTime);
                    case 2:
                        return searchQuery.OrderBy(y => y.Extension);
                    case 3:
                        return searchQuery.OrderBy(y => y.LastWriteTime);
                    case 4:
                        return searchQuery.OrderBy(y => y.Name);
                }
            /// Descending
            case 1:
                switch (FileSort)
                {
                    default:
                    case 0:
                        return searchQuery.OrderByDescending(y => y.LastAccessTime);
                    case 1:
                        return searchQuery.OrderByDescending(y => y.CreationTime);
                    case 2:
                        return searchQuery.OrderByDescending(y => y.Extension);
                    case 3:
                        return searchQuery.OrderByDescending(y => y.LastWriteTime);
                    case 4:
                        return searchQuery.OrderByDescending(y => y.Name);
                }
        }
    }

    private string getFileExtension(string fileExtension)
    {
        fileExtension = fileExtension.StartsWith(".") ? fileExtension.Substring(1) : fileExtension;
        switch (FileExtension)
        {
            default:
            case 0:
                return "." + fileExtension;
            case 1:
                return "." + char.ToUpper(fileExtension[0]) + fileExtension.Substring(1);
            case 2:
                return "." + fileExtension.ToLower();
            case 3:
                return "." + fileExtension.ToUpper();
        }
    }

    private string getFileName(int index)
    {
        switch (FileNameIncrementBase)
        {
            ///  (2) Binary
            case 0:
                return getBase(index, new char[] { '0', '1' });
            /// (10) Decimal
            case 1:
                return index.ToString();
            /// (16) Hexadecimal
            case 2:
                return getBase(index, new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' });

            default: goto case 1;
        }
    }

    ///

    public override void OnSetProperty(PropertySetEventArgs e)
    {
        base.OnSetProperty(e);
        switch (e.PropertyName)
        {
            case nameof(FileExtension):
            case nameof(FileNameIncrement):
            case nameof(FileNameIncrementBase):
            case nameof(FileNameIncrementBy):
            case nameof(FileNameIncrementStart):
            case nameof(FileSort):
            case nameof(FileSortDirection):
            case nameof(FolderPath):
                Preview();
                break;
        }
    }

    ///

    public void Preview()
    {
        Files.Clear();

        var folder = new DirectoryInfo(FolderPath);

        int.TryParse(FileNameIncrementStart, out int count);
        int.TryParse(FileNameIncrement, out int increment);

        foreach (var x in getFiles(folder))
        {
            var file = new FilePreview
            {
                OldName = x.Name,
                NewName = getFileName(count) + getFileExtension(x.Extension)
            };

            Files.Add(file);
            count += increment;
        }
    }

    ///

    [NotTested]
    async public void Do()
    {
        if (isDoing) { return; }
        isDoing = true;

        var result = await Dialog.ShowProgress("Renaming...", "Renaming. Please wait...", _ =>
        {
            var folder = new DirectoryInfo(FolderPath);
            int.TryParse(FileNameIncrementStart, out int count);
            int.TryParse(FileNameIncrement, out int increment);

            foreach (var x in getFiles(folder))
            {
                var i = getRandomString(8) + getFileExtension(x.Extension);
                var j = Path.Combine(x.Directory.FullName, i);

                System.IO.File.Move(x.FullName, j);
            }
            foreach (var x in getFiles(folder))
            {
                var h = getFileExtension(x.Extension);
                var i = getFileName(count) + h;
                var j = Path.Combine(x.Directory.FullName, i);

                while (System.IO.File.Exists(j))
                {
                    count += increment;
                    i = getFileName(count) + h;
                    j = Path.Combine(x.Directory.FullName, i);
                }

                System.IO.File.Move(x.FullName, j);
                count += increment;
            }

        }, TimeSpan.FromSeconds(3), true);

        Result = result is Success ? new Success("Files renamed!") : result;
        isDoing = false;
    }

    public ICommand DoCommand => Commands[nameof(DoCommand)] ??= new RelayCommand(() => Do(), () => true);
}