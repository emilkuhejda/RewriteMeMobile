namespace RewriteMe.Domain.Dialog
{
    public class PromptResult
    {
        public PromptResult(bool ok, string text)
        {
            Ok = ok;
            Text = text;
        }

        public bool Ok { get; }

        public string Text { get; }
    }
}
