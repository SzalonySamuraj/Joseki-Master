using System.Collections.Generic;

namespace Baduk
{
    public struct BadukStone
    {
        public int X, Y;
        public int Color;
        public BadukStone(int pX, int pY, int pColor)
        {
            X = pX;
            Y = pY;
            Color = pColor;
        }
    }

    public struct BadukDame
    {
        public int X, Y;
        public BadukDame(int pX, int pY)
        {
            X = pX;
            Y = pY;
        }
    }

    public struct BadukMove
    {
        public int X, Y, Color;
        public string Comment { get; set; }


        public BadukMove(int pColor, int pX, int pY, string comment = null)
        {
            X = pX;
            Y = pY;
            Comment = comment;
            Color = pColor;
        }


        public void Init(int pColor, int pX, int pY)
        {
            X = pX;
            Y = pY;
            Color = pColor;
        }
    }

    public struct BadukMark
    {
        public int X, Y;
        public int Type;
        public BadukMark(int pX, int pY, int pType)
        {
            X = pX;
            Y = pY;
            Type = pType;
        }
    }

    class BadukGroup
    {
        public List<BadukStone> stones;
        public List<BadukDame> liberties;
        public int Color;

        public BadukGroup(BadukBoard Board, int pX, int pY)
        {
            stones = new List<BadukStone>();
            liberties = new List<BadukDame>();
            Clear();
            InitStonesList(Board, pX, pY);
            CalculateDame(Board);
        }

        public void AddStone(int pX, int pY, int pColor)
        {
            stones.Add(new BadukStone(pX, pY, pColor));
        }

        public void Clear()
        {
            stones.Clear();
            liberties.Clear();
            Color = -1;
        }

        public void CalculateDame(BadukBoard Board)
        {
            liberties.Clear();
            for (int i = 0; i < stones.Count; i++)
            {
                bool is_slot_at_right = Board.ValidateCoords(stones[i].X + 1, stones[i].Y);
                bool is_dame_at_right = !(isStoneOnBoard(Board, stones[i].X + 1, stones[i].Y));
                bool is_slot_at_left = Board.ValidateCoords(stones[i].X - 1, stones[i].Y);
                bool is_dame_at_left = !(isStoneOnBoard(Board, stones[i].X - 1, stones[i].Y));
                bool is_slot_at_top = Board.ValidateCoords(stones[i].X, stones[i].Y - 1);
                bool is_dame_at_top = !(isStoneOnBoard(Board, stones[i].X, stones[i].Y - 1));
                bool is_slot_at_bottom = Board.ValidateCoords(stones[i].X, stones[i].Y + 1);
                bool is_dame_at_bottom = !(isStoneOnBoard(Board, stones[i].X, stones[i].Y + 1));
                if (is_slot_at_right && is_dame_at_right)
                {
                    bool is_right_already_in_list = false;
                    for (int j = 0; j < liberties.Count; j++)
                    {
                        if (liberties[j].X == stones[i].X + 1 && liberties[j].Y == stones[i].Y)
                        {
                            is_right_already_in_list = true;
                        }
                    }
                    if (is_right_already_in_list == false)
                    {
                        liberties.Add(new BadukDame(stones[i].X + 1, stones[i].Y));
                    }
                }
                if (is_slot_at_left && is_dame_at_left)
                {
                    bool is_left_already_in_list = false;
                    for (int j = 0; j < liberties.Count; j++)
                    {
                        if (liberties[j].X == stones[i].X - 1 && liberties[j].Y == stones[i].Y)
                        {
                            is_left_already_in_list = true;
                        }
                    }
                    if (is_left_already_in_list == false)
                    {
                        liberties.Add(new BadukDame(stones[i].X - 1, stones[i].Y));
                    }
                }
                if (is_slot_at_top && is_dame_at_top)
                {
                    bool is_top_already_in_list = false;
                    for (int j = 0; j < liberties.Count; j++)
                    {
                        if (liberties[j].X == stones[i].X && liberties[j].Y == stones[i].Y - 1)
                        {
                            is_top_already_in_list = true;
                        }
                    }
                    if (is_top_already_in_list == false)
                    {
                        liberties.Add(new BadukDame(stones[i].X, stones[i].Y - 1));
                    }
                }
                if (is_slot_at_bottom && is_dame_at_bottom)
                {
                    bool is_bottom_already_in_list = false;
                    for (int j = 0; j < liberties.Count; j++)
                    {
                        if (liberties[j].X == stones[i].X && liberties[j].Y == stones[i].Y + 1)
                        {
                            is_bottom_already_in_list = true;
                        }
                    }
                    if (is_bottom_already_in_list == false)
                    {
                        liberties.Add(new BadukDame(stones[i].X, stones[i].Y + 1));
                    }
                }
            }
            ;
        }

        public bool isStoneOnBoard(BadukBoard Board, int pX, int pY)
        {
            bool result = false;
            bool CoordsValidated = Board.ValidateCoords(pX, pY);
            if (CoordsValidated == false)
            {
                return false;
            }
            for (int i = 0; i < Board.stones.Count; i++)
            {
                if (Board.stones[i].X == pX && Board.stones[i].Y == pY)
                {
                    result = true;
                }
            }
            return result;
        }

        public int GetStoneColorInBoard(BadukBoard Board, int pX, int pY)
        {
            int result = Board.GetStoneColor(pX, pY);
            return result;
        }

        public bool isStoneInGroup(int pX, int pY)
        {
            bool result = false;
            for (int i = 0; i < stones.Count; i++)
            {
                if (stones[i].X == pX && stones[i].Y == pY)
                {
                    result = true;
                }
            }
            return result;
        }

        public void InitStonesList(BadukBoard Board, int pX, int pY)
        {
            Clear();
            bool WeHaveNewStonesToCheck = false;
            //Check stone in pX, pY
            for (int i = 0; i < Board.stones.Count; i++)
            {
                if (Board.stones[i].X == pX && Board.stones[i].Y == pY)
                {
                    //Stone founded
                    AddStone(Board.stones[i].X, Board.stones[i].Y, Board.stones[i].Color);
                    Color = Board.stones[i].Color;
                    WeHaveNewStonesToCheck = true;
                }
                else
                {
                    //No stone in pX, pY
                }
            }

            int IterationCount = 0;

            while (WeHaveNewStonesToCheck)
            {
                IterationCount++;

                WeHaveNewStonesToCheck = false;

                // Check all stones
                for (int i = 0; i < stones.Count; i++)
                {
                    bool is_stone_at_right = isStoneOnBoard(Board, stones[i].X + 1, stones[i].Y);
                    bool is_stone_at_left = isStoneOnBoard(Board, stones[i].X - 1, stones[i].Y);
                    bool is_stone_at_top = isStoneOnBoard(Board, stones[i].X, stones[i].Y - 1);
                    bool is_stone_at_bottom = isStoneOnBoard(Board, stones[i].X, stones[i].Y + 1);

                    if (is_stone_at_right)
                    {
                        bool in_right_stone_in_group = isStoneInGroup(stones[i].X + 1, stones[i].Y);
                        if (in_right_stone_in_group == false)
                        {
                            int right_stone_color = GetStoneColorInBoard(Board, stones[i].X + 1, stones[i].Y);
                            if (right_stone_color == Color)
                            {
                                AddStone(stones[i].X + 1, stones[i].Y, stones[i].Color);
                                WeHaveNewStonesToCheck = true;
                            }
                        }
                    }
                    if (is_stone_at_left)
                    {
                        bool in_left_stone_in_group = isStoneInGroup(stones[i].X - 1, stones[i].Y);
                        if (in_left_stone_in_group == false)
                        {
                            int left_stone_color = GetStoneColorInBoard(Board, stones[i].X - 1, stones[i].Y);
                            if (left_stone_color == Color)
                            {
                                AddStone(stones[i].X - 1, stones[i].Y, stones[i].Color);
                                WeHaveNewStonesToCheck = true;
                            }
                        }
                    }
                    if (is_stone_at_top)
                    {
                        bool in_top_stone_in_group = isStoneInGroup(stones[i].X, stones[i].Y - 1);
                        if (in_top_stone_in_group == false)
                        {
                            int top_stone_color = GetStoneColorInBoard(Board, stones[i].X, stones[i].Y - 1);
                            if (top_stone_color == Color)
                            {
                                AddStone(stones[i].X, stones[i].Y - 1, stones[i].Color);
                                WeHaveNewStonesToCheck = true;
                            }
                        }
                    }
                    if (is_stone_at_bottom)
                    {
                        bool in_bottom_stone_in_group = isStoneInGroup(stones[i].X, stones[i].Y + 1);
                        if (in_bottom_stone_in_group == false)
                        {
                            int bottom_stone_color = GetStoneColorInBoard(Board, stones[i].X, stones[i].Y + 1);
                            if (bottom_stone_color == Color)
                            {
                                AddStone(stones[i].X, stones[i].Y + 1, stones[i].Color);
                                WeHaveNewStonesToCheck = true;
                            }
                        }
                    }

                }

                if (IterationCount > 2000)
                {
                    WeHaveNewStonesToCheck = false;
                }
            }
            ; //While cycle ended
        }
    }

    class BadukBoard
    {
        public int size_x, size_y;
        public bool Enabled;

        public bool LastMoveSuccesfull = false;

        public List<BadukStone> stones;
        public List<BadukMark> marks;
        public List<BadukMove> Moves;

        public int ko_X, ko_Y;

        public int CurrentColor = 0;

        //public List<BadukMove> imported_moves;
        public int CurrentPlayerMove;

        public bool LastMoveCorrect;

        //public int Emotion;

        public BadukBoard()
        {
            stones = new List<BadukStone>();
            marks = new List<BadukMark>();
            Moves = new List<BadukMove>();
            Enabled = true;
            CurrentPlayerMove = 0;
            size_x = 19;
            size_y = 19;
            ko_X = -1;
            ko_Y = -1;
            LastMoveCorrect = true;
        }

        public void PlaceStone(int pX, int pY, int pColor)
        {
            bool CoordsValidated = ValidateCoords(pX, pY);
            if (CoordsValidated)
            {
                stones.Add(new BadukStone(pX, pY, pColor));
            }
        }

        public void MarkSlot(int pX, int pY, int pType)
        {
            bool CoordsValidated = ValidateCoords(pX, pY);
            if (CoordsValidated)
            {
                marks.Add(new BadukMark(pX, pY, pType));
            }
        }

        public void MakeMove(int pX, int pY)
        {
            bool SlotEmpty = isSlotEmpty(pX, pY);
            bool CoordsValidated = ValidateCoords(pX, pY);
            bool RescrictedMove = CheckRestrictedMove(pX, pY);

            if (SlotEmpty && CoordsValidated && !RescrictedMove)
            {
                LastMoveSuccesfull = true;
                CurrentPlayerMove++;
                Moves.Add(new BadukMove(CurrentColor, pX, pY));
                PlaceStone(pX, pY, CurrentColor);
                stones.ToString();
                CheckNearDeadGroups(pX, pY);
                ChangeCurrentColor();
            }
            else
            {
                LastMoveSuccesfull = false;
            }
        }

        public void RemoveStone(int pX, int pY)
        {
            for (int i = 0; i < stones.Count; i++)
            {
                if (stones[i].X == pX && stones[i].Y == pY)
                {
                    stones.RemoveAt(i);
                    i = 0; //Reset iterator to find duplicated stones
                }
            }
        }

        public int GetStoneColor(int pX, int pY)
        {
            int result = -1;
            for (int i = 0; i < stones.Count; i++)
            {
                if (stones[i].X == pX && stones[i].Y == pY)
                {
                    result = stones[i].Color;
                }
            }
            return result;
        }

        public void SetKoPosition(int pX, int pY)
        {
            ko_X = pX;
            ko_Y = pY;
        }

        public void ClearKoPosition()
        {
            ko_X = -1;
            ko_Y = -1;
        }

        public void ChangeCurrentColor()
        {
            if (CurrentColor == 0)
            {
                CurrentColor = 1;
            }
            else
            {
                CurrentColor = 0;
            }
        }

        public bool isSlotEmpty(int pX, int pY)
        {
            bool CoordsValidated = ValidateCoords(pX, pY);

            // Slot is not empty if out of board
            if (CoordsValidated == false)
            {
                return false;
            }

            for (int i = 0; i < stones.Count; i++)
            {
                if (stones[i].X == pX && stones[i].Y == pY)
                {
                    return false;
                }
            }
            return true;
        }

        public bool ValidateCoords(int pX, int pY)
        {
            if (pX >= 0 && pX < size_x && pY >= 0 && pY < size_y)
            {
                return true;
            }
            return false;
        }

        public void Clear()
        {
            ClearStones();
            ClearKoPosition();
            ClearMarks();
            CurrentColor = 0;
            CurrentPlayerMove = 0;
        }

        public void ClearMarks()
        {
            marks.Clear();
        }

        public void ClearStones()
        {
            stones.Clear();
        }

        public void RemoveGroupFromBoard(BadukGroup Group)
        {
            for (int i = 0; i < Group.stones.Count; i++)
            {
                RemoveStone(Group.stones[i].X, Group.stones[i].Y);
            }
        }

        public void CheckNearDeadGroups(int pX, int pY)
        {
            BadukGroup TempGroup;
            //Right group
            TempGroup = new BadukGroup(this, pX + 1, pY);
            if (TempGroup.liberties.Count == 0)
            {
                RemoveGroupFromBoard(TempGroup);
            }
            //Left group
            TempGroup = new BadukGroup(this, pX - 1, pY);
            if (TempGroup.liberties.Count == 0)
            {
                RemoveGroupFromBoard(TempGroup);
            }
            //Top group
            TempGroup = new BadukGroup(this, pX, pY - 1);
            if (TempGroup.liberties.Count == 0)
            {
                RemoveGroupFromBoard(TempGroup);
            }
            //Bottom group
            TempGroup = new BadukGroup(this, pX, pY + 1);
            if (TempGroup.liberties.Count == 0)
            {
                RemoveGroupFromBoard(TempGroup);
            }
        }

        public bool CheckRestrictedMove(int pX, int pY)
        {
            if (ko_X == pX && ko_Y == pY)
            {
                return true;
            }

            int EnemyColor;
            if (CurrentColor == 1)
            {
                EnemyColor = 0;
            }
            else
            {
                EnemyColor = 1;
            }

            // Additional check
            bool is_my_slot_free = isSlotEmpty(pX, pY);

            bool is_free_slot_at_right = isSlotEmpty(pX + 1, pY);
            bool is_free_slot_at_left = isSlotEmpty(pX - 1, pY);
            bool is_free_slot_at_top = isSlotEmpty(pX, pY - 1);
            bool is_free_slot_at_bottom = isSlotEmpty(pX, pY + 1);

            if (is_my_slot_free)
            {
                if (is_free_slot_at_right || is_free_slot_at_left || is_free_slot_at_top || is_free_slot_at_bottom)
                {
                    ClearKoPosition();
                    return false;
                }

                BadukGroup TempGroup;
                //Check right weak group;
                TempGroup = new BadukGroup(this, pX + 1, pY);
                if (TempGroup.Color == EnemyColor && TempGroup.liberties.Count == 1)
                {
                    ClearKoPosition();
                    if (TempGroup.stones.Count == 1)
                    {
                        SetKoPosition(pX + 1, pY);
                    }
                    return false;
                }
                //Check left weak group;
                TempGroup = new BadukGroup(this, pX - 1, pY);
                if (TempGroup.Color == EnemyColor && TempGroup.liberties.Count == 1)
                {
                    ClearKoPosition();
                    if (TempGroup.stones.Count == 1)
                    {
                        SetKoPosition(pX - 1, pY);
                    }
                    return false;
                }
                //Check top weak group;
                TempGroup = new BadukGroup(this, pX, pY - 1);
                if (TempGroup.Color == EnemyColor && TempGroup.liberties.Count == 1)
                {
                    ClearKoPosition();
                    if (TempGroup.stones.Count == 1)
                    {
                        SetKoPosition(pX, pY - 1);
                    }
                    return false;
                }
                //Check bottom weak group;
                TempGroup = new BadukGroup(this, pX, pY + 1);
                if (TempGroup.Color == EnemyColor && TempGroup.liberties.Count == 1)
                {
                    ClearKoPosition();
                    if (TempGroup.stones.Count == 1)
                    {
                        SetKoPosition(pX, pY + 1);
                    }
                    return false;
                }

                bool is_group_dead_after_move = isGroupDeadAfterMove(pX, pY);
                if (is_group_dead_after_move)
                {
                    return true;
                }
            }
            ClearKoPosition();
            return false;
        }

        public bool isGroupDeadAfterMove(int pX, int pY)
        {
            bool is_slot_free = isSlotEmpty(pX, pY);
            if (!is_slot_free)
            {
                return false;
            }
            else
            {
                PlaceStone(pX, pY, CurrentColor);
                BadukGroup TempGroup = new BadukGroup(this, pX, pY);
                RemoveStone(pX, pY);
                if (TempGroup.liberties.Count == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void Enable()
        {
            Enabled = true;
        }

        public void Disable()
        {
            Enabled = false;
        }
    }
}
