namespace Kiss.Bff.EndToEndTest.Helpers
{
    public static class AcceptAllDialogsExtension
    {
        public static IDisposable AcceptAllDialogs(this IPage page)
        {
            page.Dialog += Accept;
            return new DoOnDispose(() => page.Dialog -= Accept);
        }

        private static async void Accept(object? _, IDialog dialog) => await dialog.AcceptAsync();


        private sealed class DoOnDispose(Action action) : IDisposable
        {
            public void Dispose() => action();
        }
    }
}
