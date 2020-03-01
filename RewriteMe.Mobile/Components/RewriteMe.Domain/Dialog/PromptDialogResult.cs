namespace RewriteMe.Domain.Dialog
{
    public class PromptDialogResult
    {
        public PromptDialogResult(bool ok, string text)
        {
            Ok = ok;
            Text = text;
        }

        public bool Ok { get; }

        public string Text { get; }
    }
}
