using Ion.Core;

namespace Ion.Tools.Rename;

public record class FilePreview : Model
{
    public String OldName { get => Get(""); set => Set(value); }

    public String NewName { get => Get(""); set => Set(value); }
}
