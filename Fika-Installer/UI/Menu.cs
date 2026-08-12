namespace Fika_Installer.UI;

public class Menu
{
    public string Message { get; }
    public List<MenuChoice> Choices { get; }

    private int _menuStartPos;
    private int _paginationIndex;

    public Menu(List<MenuChoice> choices)
    {
        Message = "";
        Choices = choices;
    }

    public Menu(string message, List<MenuChoice> choices)
    {
        Message = message;
        Choices = choices;
    }

    public MenuChoice Show()
    {
        _menuStartPos = Console.CursorTop;

        while (true)
        {
            var paging = Choices.Count > 9;
            var pageSize = paging ? 8 : 9;

            if (!string.IsNullOrWhiteSpace(Message))
            {
                Console.WriteLine(Message);
                Console.WriteLine();
            }

            var remaining = Choices.Count - _paginationIndex;
            var pageChoiceCount = Math.Min(pageSize, remaining);
            var nextChoiceNumber = pageChoiceCount + 1;

            for (var i = 0; i < pageChoiceCount; i++)
            {
                var choiceText = Choices[_paginationIndex + i].Text;
                Console.WriteLine($"[{i + 1}] {choiceText}");
            }

            if (paging)
            {
                Console.WriteLine($"[{nextChoiceNumber}] Next");
            }

            var keyInfoPressed = Console.ReadKey(true);

            var keyPressed = keyInfoPressed.KeyChar.ToString();

            if (int.TryParse(keyPressed, out var choiceNumber))
            {
                if (paging)
                {
                    if (choiceNumber == nextChoiceNumber)
                    {
                        ClearMenu();

                        _paginationIndex += pageChoiceCount;

                        if (_paginationIndex >= Choices.Count)
                        {
                            _paginationIndex = 0;
                        }

                        continue;
                    }
                }

                if (choiceNumber >= 1 && choiceNumber <= pageChoiceCount)
                {
                    ClearMenu();

                    var choiceIndex = _paginationIndex + choiceNumber - 1;
                    var choice = Choices[choiceIndex];

                    choice.Execute();

                    return choice;
                }
            }

            ClearMenu();
        }
    }

    private void ClearMenu()
    {
        var currentPos = Console.CursorTop;

        for (var i = currentPos; i >= _menuStartPos; i--)
        {
            Console.SetCursorPosition(0, i);
            Console.Write(new string(' ', Console.WindowWidth));
        }

        Console.SetCursorPosition(0, _menuStartPos);
    }
}
