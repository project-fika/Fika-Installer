namespace Fika_Installer.UI
{    
    public abstract class Page
    {
        private int _totalLines = 0;
        private ILogger _logger;
        private IPageLogger _pageLogger;

        public CompositeLogger PageLogger { get; private set; }

        public Page(ILogger logger)
        {
            _logger = logger;
            _pageLogger = new PageLogger(AddLines);

            PageLogger = new();
            PageLogger.AddLogger(_logger);
            PageLogger.AddLogger(_pageLogger);
        }
        
        public void Show()
        {
            OnShow();
            Dispose();
        }

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

        public abstract void OnShow();

        private void AddLines(string message, bool confirm = false)
        {
            _totalLines++;

            if (confirm)
            {
                _totalLines += 2;
            }
        }
    }
}