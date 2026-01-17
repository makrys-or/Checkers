using System;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;

namespace checkers
{
    class Utils
    {
        public static void CheckersPrint(Board board)
        {    
            string[] chars = new string[8] {"   a", "    b", "    c", "    d", "    e", "    f", "    g", "    h"};

            for (int i = 0; i < 8; i++)
            {
                System.Console.Write(chars[i]);
            }
            System.Console.Write("\n");

            for (int row = 8; row > 0; row--)
                {
                    System.Console.Write(row);

                    for (int col = 1; col < 9; col++)
                    {   
                        var cell = board.Cells[row,col];
                        if (cell.Checker != null)
                        {
                            if (cell.Checker.IsKing == false)// если просто шашка
                            {
                                if (cell.Checker.Colour == PieceColor.White)
                                System.Console.Write("[ o ]");
                                else
                                System.Console.Write("[ x ]");
                            }

                            if (cell.Checker.IsKing == true)// если дамка 
                            {
                                if (cell.Checker.Colour == PieceColor.White)
                                System.Console.Write("[ O ]");
                                else
                                System.Console.Write("[ X ]");
                            }
                            
                        }
                        else if (cell.IsPlayable == false)
                        {
                            System.Console.Write("|   |");
                        }

                        else 
                            System.Console.Write("[   ]");
                        
                    }
                    System.Console.Write(row);
                    System.Console.Write("\n");
                }
                for (int i = 0; i < 8; i++)
                {
                    System.Console.Write(chars[i]);
                }
                System.Console.Write("\n");

            // for (int row = 8; row > 0; row--)
            //     {
            //         for (int col = 1; col < 9; col++)
            //         {   
            //             var cell = board.Cells[row,col];
            //             if (cell.Checker != null)
            //             {
            //                 System.Console.Write($"Шашка {row},{col} ");
            //                 if (cell.Checker.Colour == PieceColor.White)
            //                 System.Console.WriteLine("белая");
            //                 if (cell.Checker.Colour == PieceColor.Black)
            //                 System.Console.WriteLine("черная");
            //             }
            //             else System.Console.WriteLine($"На клетке {row},{col} нет шашки");
            //         }
            //     }
        }
        public void InitializeCheckers(Board board)
        {
            var Cells = board.Cells;
            
            for (int row = 6; row < 9; row++)
            {
                for (int col = 1; col < 9; col++)
                {
                    if (Cells[row, col].IsPlayable)//if ((row + col) % 2 == 1) // только на тёмных клетках
                    {
                        Cells[row, col].Checker = new Checker {Colour = PieceColor.White, IsKing = false};
                        board.countWhite++;
                    }
                }
            }

            for (int row = 1; row < 4; row++)
            {
                for (int col = 1; col < 9; col++)
                {
                    if (Cells[row, col].IsPlayable)//if ((row + col) % 2 == 1) // только на тёмных клетках
                    {
                        Cells[row, col].Checker = new Checker {Colour = PieceColor.Black, IsKing = false};
                        board.countBlack++;
                    }
                }
            }
            
        }

        public static int LetterToColumn(char letter)
        {
            letter = char.ToLower(letter); // на случай заглавной буквы            
            return letter - 'a' + 1;
        }
    }
}