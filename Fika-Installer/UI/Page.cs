namespace Fika_Installer.UI
{
    public abstract class Page
    {
        public ILogger FileLogger { get; set; }
        public IPageLogger PageLogger { get; set; }
        public CompositeLogger CompositeLogger { get; private set; }

        private int _pageStartPos = 0;

        public Page(ILogger logger)
        {
            FileLogger = logger;
            PageLogger = new PageLogger();

            CompositeLogger = new();
            CompositeLogger.AddLogger(FileLogger);
            CompositeLogger.AddLogger(PageLogger);

            _pageStartPos = Console.CursorTop;
        }

        public void Show()
        {
            OnShow();
            Dispose();
        }

        public abstract void OnShow();

        public void Dispose()
        {
            int currentPos = Console.CursorTop;

            for (int i = currentPos; i >= _pageStartPos; i--)
            {
                Console.SetCursorPosition(0, i);
                Console.Write(new string(' ', Console.WindowWidth));
            }

            Console.SetCursorPosition(0, _pageStartPos);
        }
    }
}