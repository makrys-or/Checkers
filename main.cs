using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace checkers
{
    class Program
    {    
        static void Main()
        {
            var board = new Board();
            var logic = new Logic();

            // System.Console.WriteLine($"Кол-во белых: {board.countWhite}.\nКол-во черных: {board.countBlack}.");

            while (board.countWhite != 0 && board.countBlack != 0)
            {
                Utils.CheckersPrint(board);

                if (logic.Turn == PieceColor.White)
                    System.Console.WriteLine("Ход белых [ o ]");
                if (logic.Turn == PieceColor.Black)
                    System.Console.WriteLine("Ход черных [ x ]");
                
                string? Act;

                // ВВОД В ЦИКЛ, ЧТОБЫ ПОВТОРЯЛСЯ ПРИ ОШИБКЕ
                while (true)
                {
                    System.Console.WriteLine("Введите ход в формате: {начальная_клетка(b6)} {конечная_клетка(a5)}");
                    Act = Console.ReadLine(); 
                    if (Act != null)
                    {
                        string[] action = Act.Split(" ");
                        if (action.Length != 2)
                        {
                            System.Console.WriteLine("logic.Action(board, Act!) = " + logic.Action(board, Act!));
                            System.Console.WriteLine("Ошибка ввода, попробуйте еще раз");
                        }
                        else break;
                        if (logic.Action(board, Act!) == false)
                        {
                            System.Console.WriteLine("logic.Action(board, Act!) = " + logic.Action(board, Act!));
                            System.Console.WriteLine("Ошибка ввода, попробуйте еще раз");
                            
                        }
                        else break;    
                    }
                }

                try
                {
                    logic.Action(board, Act!);
                    logic.SwapTurn();
                }
                catch (NullReferenceException)
                {
                   System.Console.WriteLine("Клетка пуста");
                }
                // logic.Action(board, Act!);
                // logic.SwapTurn();            
            }

            if (board.countWhite == 0)
            {
                System.Console.WriteLine("Победа черных!");
            }
            if (board.countBlack == 0)
            {
                System.Console.WriteLine("Победа белых!");
            }
            
        }
        
    }
}
