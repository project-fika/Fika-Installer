namespace Fika_Installer.UI
{
    public abstract class Page
    {
        private int _totalLines = 0;

        public ILogger FileLogger { get; set; }
        public IPageLogger PageLogger {  get; set; }
        public CompositeLogger CompositeLogger { get; private set; }

        public Page(ILogger logger)
        {
            FileLogger = logger;
            PageLogger = new PageLogger(AddLines);

            CompositeLogger = new();
            CompositeLogger.AddLogger(FileLogger);
            CompositeLogger.AddLogger(PageLogger);
        }

        public void Show()
        {
            OnShow();
            Dispose();
        }

        public abstract void OnShow();

        public void Dispose()
        {
            int currentLine = Console.CursorTop;

            for (int i = 0; i < _totalLines; i++)
            {
                Console.SetCursorPosition(0, currentLine - i - 1);
                Console.Write(new string(' ', Console.WindowWidth));
            }

            Console.SetCursorPosition(0, currentLine - _totalLines);

            _totalLines = 0;
        }

        private void AddLines(string message, bool confirm = false)
        {
            _totalLines += message.Length / Console.BufferWidth + 1;

            if (confirm)
            {
                _totalLines += 2;
            }
        }
    }
}