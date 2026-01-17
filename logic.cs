using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Assemblies;

namespace checkers
{
    class Logic
    {

        public PieceColor Turn { get; set; } = PieceColor.White;
        public void SwapTurn()
        {
            if (Turn == PieceColor.White)
            {
                Turn = PieceColor.Black;
            }
            else if (Turn == PieceColor.Black)
            {
                Turn = PieceColor.White;
            }
        }

        public void Action(Board board, string Act)
        {
            string[] actions = Act.Split(" ");//[b2; c3] 
            int SubFirstCoord = Convert.ToInt32(actions[0][1]) - '0';
            int SubSecondCoord = Utils.LetterToColumn(Convert.ToChar(actions[0][0]));
            int ObjFirstCoord = Convert.ToInt32(actions[1][1]) - '0';
            int ObjSecondCoord = Utils.LetterToColumn(Convert.ToChar(actions[1][0]));

            if (Math.Abs(SubFirstCoord - ObjFirstCoord) == 1) // если просто ход шашкой 
            {
                // Проверка правильности хода 
                if(CheckActMove(board, actions).Item1)

                {
                    var SubCell = board.Cells[SubFirstCoord, SubSecondCoord];

                    var from = board.Cells[SubFirstCoord, SubSecondCoord];
                    var to = board.Cells[ObjFirstCoord, ObjSecondCoord];
                    to.Checker = from.Checker;
                    from.Checker = null;

                    if ((to.Checker!.Colour == PieceColor.White && ObjFirstCoord == 1) || (to.Checker.Colour == PieceColor.Black && ObjFirstCoord == 8))//проверка на становление дамкой
                    {
                        to.Checker.IsKing = true;
                    }
                }
            }

            else if (Math.Abs(SubFirstCoord - ObjFirstCoord) == 2) // если рубка шашкой
            {
                if(CheckActKill(board, actions))
                {
                    int VictimFirstCoord = Math.Min(SubFirstCoord, ObjFirstCoord) + 1;
                    int VictimSecondCoord = Math.Min(SubSecondCoord, ObjSecondCoord) + 1;

                    var SubCell = board.Cells[SubFirstCoord, SubSecondCoord];
                    var ObjCell = board.Cells[ObjFirstCoord, ObjSecondCoord];
                    var VictimCell = board.Cells[VictimFirstCoord, VictimSecondCoord];

                    var from = SubCell;
                    var to = ObjCell;
                    to.Checker = from.Checker;
                    from.Checker = null;
                    VictimCell.Checker = null;

                    if ((to.Checker!.Colour == PieceColor.White && ObjFirstCoord == 1) || (to.Checker.Colour == PieceColor.Black && ObjFirstCoord == 8))//проверка на становление дамкой
                    {
                        to.Checker.IsKing = true;
                    }
                }
            }

            else 
            {
                if(CheckActMove(board, actions).Item1)
                {
                    var VictimCells = CheckActMove(board, actions).Item2;
                    var SubCell = board.Cells[SubFirstCoord, SubSecondCoord];
                    var ObjCell = board.Cells[ObjFirstCoord, ObjSecondCoord];

                    if (VictimCells.Count == 0)
                    {
                        var from = SubCell;
                        var to = ObjCell;
                        to.Checker = from.Checker;
                        from.Checker = null;
                    }
                    if (VictimCells.Count == 1)
                    {
                        var from = SubCell;
                        var to = ObjCell;
                        to.Checker = from.Checker;
                        from.Checker = null;

                        var VictimCell = board.Cells[VictimCells[0].row, VictimCells[0].col];
                        VictimCell.Checker = null;
                    }
                    
                }
            } 
        }

        // Проверка на возможность перемещения
        public (bool, List<(int row, int col)>) CheckActMove(Board board, string[] actions)
        {
            bool Output = false;

            int SubFirstCoord = Convert.ToInt32(actions[0][1]) - '0';
            int SubSecondCoord = Utils.LetterToColumn(Convert.ToChar(actions[0][0]));
            int ObjFirstCoord = Convert.ToInt32(actions[1][1]) - '0';
            int ObjSecondCoord = Utils.LetterToColumn(Convert.ToChar(actions[1][0]));

            var SubCell = board.Cells[SubFirstCoord, SubSecondCoord];
            var ObjCell = board.Cells[ObjFirstCoord, ObjSecondCoord];

            var VictimCells = new List<(int row, int col)>();

            if (SubCell.IsPlayable && ObjCell.IsPlayable)//играбельные клетки
            {
                if (SubCell.Checker!.Colour == Turn)//правильная очередь хода
                {

                    if (SubCell.Checker.IsKing == false)//если обычная шашка
                    {
                        if (Math.Abs(SubFirstCoord - ObjFirstCoord) == 1)
                        {
                            if (Math.Abs(SubSecondCoord - ObjSecondCoord) == 1)
                            {
                                if((Turn == PieceColor.White && SubFirstCoord > ObjFirstCoord) || (Turn == PieceColor.Black && SubFirstCoord < ObjFirstCoord))
                                {
                                    if(ObjCell.Checker == null)//если конечная свободна
                                        Output = true;
                                    else{System.Console.WriteLine("Клетка занята");}
                                }
                                else System.Console.WriteLine("Шашка не может ходить назад");
                            }
                            else{System.Console.WriteLine("Шашка не может так ходить");}
                        }
                        else{System.Console.WriteLine("Обычная шашка не может так ходить");}
                    }

                    
                    else if(SubCell.Checker.IsKing)// если дамка
                    {
                        if (Math.Abs(SubSecondCoord - ObjSecondCoord) == Math.Abs(SubFirstCoord - ObjFirstCoord))//ход по диагонали
                        {
                            if(ObjCell.Checker == null)//если конечная свободна
                            {
                                var (Out, list) = KingCheck(board, actions);//разбиваем выход функции на отдельные переменные(для удобства передачи)
                                
                                VictimCells = list;

                                if(Out)//общая проверка
                                {
                                    Output = true;
                                }
                                else System.Console.WriteLine("На пути несколько шашек.");
                            }
                            else{System.Console.WriteLine("Клетка занята");}
                        }
                        else System.Console.WriteLine("Это движение не по диагонали");
                    } 
                }
                else{System.Console.WriteLine("Ход не тем цветом");}
            }
            else{System.Console.WriteLine("Выбраны не игровые клетки");}

            

            return (Output, VictimCells);
        }

        // Проверка на возможнсть рубки обычной шашки
        public bool CheckActKill(Board board, string[] actions)
        {
            bool Output = false;

            int SubFirstCoord = Convert.ToInt32(actions[0][1]) - '0';
            int SubSecondCoord = Utils.LetterToColumn(Convert.ToChar(actions[0][0]));
            int ObjFirstCoord = Convert.ToInt32(actions[1][1]) - '0';
            int ObjSecondCoord = Utils.LetterToColumn(Convert.ToChar(actions[1][0]));

            int VictimFirstCoord = (SubFirstCoord + ObjFirstCoord) / 2;
            int VictimSecondCoord = (SubSecondCoord + ObjSecondCoord) / 2;

            var SubCell = board.Cells[SubFirstCoord, SubSecondCoord];
            var ObjCell = board.Cells[ObjFirstCoord, ObjSecondCoord];
            var VictimCell = board.Cells[VictimFirstCoord, VictimSecondCoord];
        

            if (SubCell.IsPlayable && ObjCell.IsPlayable)
            {
                if (SubCell.Checker!.Colour == Turn)
                {
                    if (SubCell.Checker.IsKing == false)
                    {
                        if (Math.Abs(SubFirstCoord - ObjFirstCoord) == 2)
                        {
                            if (Math.Abs(SubSecondCoord - ObjSecondCoord) == 2)
                            {
                                if(ObjCell.Checker == null && VictimCell.Checker != null)
                                {
                                    if(VictimCell.Checker.Colour != SubCell.Checker.Colour)
                                    {
                                        Output = true;
                                    }
                                    else System.Console.WriteLine("Ты зачем своего рубишь?");
                                }
                                else{System.Console.WriteLine("Клетка занята или нет жертвы");}
                            }
                            else{System.Console.WriteLine("Шашка не может так ходить");}
                        }
                        else{System.Console.WriteLine("Обычная шашка не может так ходить");}
                    }
                    
                }
                else{System.Console.WriteLine("Ход не тем цветом");}
            }
            else{System.Console.WriteLine("Выбраны не игровые клетки");}
            
            return Output;
        } 


        // Проверка, есть ли на пути у дамки другие шашки
        public (bool, List<(int row, int col)>) KingCheck(Board board, string[] actions)//не доделал
        {
            bool Out = true;
            
            int SubFirstCoord = Convert.ToInt32(actions[0][1]) - '0';
            int SubSecondCoord = Utils.LetterToColumn(Convert.ToChar(actions[0][0]));
            int ObjFirstCoord = Convert.ToInt32(actions[1][1]) - '0';
            int ObjSecondCoord = Utils.LetterToColumn(Convert.ToChar(actions[1][0]));

            var SubCell = board.Cells[SubFirstCoord, SubSecondCoord];
            var ObjCell = board.Cells[ObjFirstCoord, ObjSecondCoord];

            // d8 h4 (f6 стоит враг)   8,4 4,8 (6,6 враг)

            //начинаем подозрительную клетку от положения дамки
            int SusFC = SubFirstCoord;
            int SusSC = SubSecondCoord; 
            var VictimCells = new List<(int row, int col)>();// список найденных на пути шашек

            for (int i = SusFC; i != ObjFirstCoord;)// перечисляем подозрительные клетки
            {

                if (SubFirstCoord > ObjFirstCoord)//если дамка выше
                {
                    i -= 1;
                    if (SubSecondCoord > ObjSecondCoord)//если дамка правее 
                    {
                        SusSC -= 1;
                    }
                    else {SusSC += 1;}//если дамка левее
                }

                if (SubFirstCoord < ObjFirstCoord)//если дамка ниже
                {
                    i += 1;
                    if (SubSecondCoord > ObjSecondCoord)//если дамка правее 
                    {
                        SusSC -= 1;
                    }
                    else {SusSC += 1;}//если дамка левее
                }

                if (board.Cells[i, SusSC].Checker != null)//если подозрительная клетка непустая 
                {
                    VictimCells.Add((i, SusSC));
                    System.Console.WriteLine("DEBAG:На пути дамки обнаружена шашка");

                    if (board.Cells[i, SusSC].Checker!.Colour == SubCell.Checker!.Colour)//через свою шашку ходить нельзя
                    {
                        Out = false;
                        break;
                    }
                }

                //дамка не может ходить, если на пути 2 шашки или больше
                if (VictimCells.Count == 2)
                {
                    Out = false;
                    break;
                }
            }

            return (Out, VictimCells);
        }
    }
}