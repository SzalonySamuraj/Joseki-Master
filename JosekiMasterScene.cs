using System;
using System.Text;
using System.Windows.Threading;
using Baduk;
using Microsoft.Win32;

namespace JosekiMaster
{
    class JosekiMasterScene
    {
        public BadukBoard Board = new BadukBoard();
        public WPFBadukBoardRenderer BoardRenderer = new WPFBadukBoardRenderer();
        public JosekiCollection JosekiList = new JosekiCollection();
        public BadukJoseki TempJoseki = new BadukJoseki();

        public string CollectionFilenameWithPath = "";

        public bool BoardEnabled = false;
        public bool BoardVisible = true;

        public string State = "started";

        int CurrentJoseki = -1;
        int CurrentMove = 0;
        public bool JosekiFinished = false;
        public bool PlayerFirstMove = true;
        public bool LastMoveCorrect = true;

        DispatcherTimer MakeNextMoveTimer;
        DispatcherTimer RepeatJosekiTimer;

        public bool ResultPositionDemo = false;

        public int Click_X = -1, Click_Y = -1;
        public bool ShiftPressed = false;

        public string TitleText = null;
        public string TopLeftText = "";
        public string TopRightText = "";

        public string CharacterEmotion = "liza-neutral";
        public int CharacterWidth = 600, CharacterHeight = 400;

        public int ClientWidth = 1600, ClientHeight = 900;
        public int FontSize = 16;

        public JosekiMasterScene()
        {
            BoardRenderer.InitBoard(Board);
            UpdateInfoBlock();
        }

        public void InitTimers(DispatcherTimer pMakeNextMoveTimer, DispatcherTimer pRepeatJosekiTimer)
        {
            MakeNextMoveTimer = pMakeNextMoveTimer;
            RepeatJosekiTimer = pRepeatJosekiTimer;
        }

        public void LeftClick(int pX, int pY)
        {
            if (BoardEnabled && !ResultPositionDemo)
            {
                CalculateClickPosition(pX, pY);
            }

            if (State == "training")
            {
                if (ResultPositionDemo)
                {
                    ResultPositionDemo = false;
                    ResetPosition();
                }
                else
                {
                    MakeMove(Click_X, Click_Y);
                    if (LastMoveCorrect && !JosekiFinished)
                    {
                        BoardEnabled = false;
                        MakeNextMoveTimer.Start();
                    }
                }
            }
            if (State == "learning")
            {
                if (ResultPositionDemo)
                {
                    ResultPositionDemo = false;
                    ResetPosition();
                }
                else
                {
                    MakeMove(Click_X, Click_Y);
                    if (LastMoveCorrect)
                    {
                        UpdateHint();
                    }
                }
            }

            if (State == "editor-starting-stones")
            {
                if (ShiftPressed)
                {
                    TempJoseki.StartingStones.Add(new BadukStone(Click_X, Click_Y, 1));
                    Board.PlaceStone(Click_X, Click_Y, 1);
                }
                else
                {
                    TempJoseki.StartingStones.Add(new BadukStone(Click_X, Click_Y, 0));
                    Board.PlaceStone(Click_X, Click_Y, 0);
                }
            }

            if (State == "editor-writing-moves")
            {
                MakeMove(Click_X, Click_Y);
            }

            UpdateInfoBlock();
        }

        public void CalculateClickPosition(int pX, int pY)
        {
            WPFBadukBoardRenderer br = BoardRenderer;
            Click_X = -1;
            Click_Y = -1;
            int GridPositionX = (int)(pX - br.pos_x - br.margin + 0.5 * br.cell_size);
            int GridPositionY = (int)(pX - br.pos_x - br.margin + 0.5 * br.cell_size);

            if (GridPositionX >= 0 && GridPositionY >= 0)
            {
                int temp_Click_X = (int)((pX - br.pos_x - br.margin + 0.5 * br.cell_size) / br.cell_size);
                int temp_Click_Y = (int)((pY - br.pos_y - br.margin + 0.5 * br.cell_size) / br.cell_size);
                if (temp_Click_X >= 0 && temp_Click_X < Board.size_x && temp_Click_Y >= 0 && temp_Click_Y < Board.size_y)
                {
                    Click_X = temp_Click_X;
                    Click_Y = temp_Click_Y;
                }
            }
        }

        public void SetState(string param)
        {
            BoardVisible = true;
            SetEmotion("liza-neutral");

            if (param == "training")
            {
                State = "training";
                IncrementIndex();
                ShowResultPosition();
            }
            if (param == "learning")
            {
                State = "learning";
                ResetPosition();
                ResultPositionDemo = false;
            }
            if (param == "editor-overview")
            {
                State = "editor-overview";
                ShowResultPosition();
            }
            if (param == "editor-starting-stones")
            {
                State = "editor-starting-stones";
            }
            if (param == "editor-writing-moves")
            {
                State = "editor-writing-moves";
            }
            if (param == "about")
            {
                SetEmotion("liza-happy");
                BoardVisible = false;
                ResultPositionDemo = false;
                State = "about";
            }
            UpdateInfoBlock();
        }

        public void OpenJosekiCollection()
        {
            BoardEnabled = false;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Joseki Collection files|*.josekicollection";
            if (openFileDialog.ShowDialog() == true)
            {
                JosekiList.Import(openFileDialog.FileName);
                CollectionFilenameWithPath = openFileDialog.FileName;
                SetState("training");
            }
            UpdateHint();
            UpdateInfoBlock();
        }

        public void IncrementIndex()
        {
            if (State == "training")
            {
                Random rnd = new Random();
                int NewIndex = rnd.Next(JosekiList.JosekiList.Count);
                if (NewIndex == CurrentJoseki)
                {
                    // Second try to not repeat
                    NewIndex = rnd.Next(JosekiList.JosekiList.Count);
                    if (NewIndex == CurrentJoseki)
                    {
                        // One last try
                        CurrentJoseki = rnd.Next(JosekiList.JosekiList.Count);
                    }
                    else
                    {
                        CurrentJoseki = NewIndex;
                    }
                }
                else
                {
                    CurrentJoseki = NewIndex;
                }
            }
            if (State == "learning")
            {
                CurrentJoseki++;
                if (CurrentJoseki >= JosekiList.JosekiList.Count)
                {
                    CurrentJoseki = 0;
                }
            }
            if (State == "editor-overview")
            {
                if (CurrentJoseki < JosekiList.JosekiList.Count - 1)
                {
                    CurrentJoseki++;
                }
            }
        }

        public void DecrementIndex()
        {
            if (CurrentJoseki > 0)
            {
                CurrentJoseki--;
            }
        }

        public void ShowResultPosition()
        {
            Board.Clear();
            if (JosekiList.JosekiList.Count == 0)
            {
                return;
            }

            if (State == "training" || State == "learning" || State == "editor-overview")
            {
                for (int i = 0; i < JosekiList.JosekiList[CurrentJoseki].StartingStones.Count; i++)
                {
                    Board.PlaceStone(JosekiList.JosekiList[CurrentJoseki].StartingStones[i].X,
                                        JosekiList.JosekiList[CurrentJoseki].StartingStones[i].Y,
                                        JosekiList.JosekiList[CurrentJoseki].StartingStones[i].Color);
                }
                if (JosekiList.JosekiList[CurrentJoseki].StartingStones.Count > 0)
                {
                    Board.CurrentColor = 1;
                }
                for (int i = 0; i < JosekiList.JosekiList[CurrentJoseki].Moves.Count; i++)
                {
                    Board.MakeMove(JosekiList.JosekiList[CurrentJoseki].Moves[i].X, JosekiList.JosekiList[CurrentJoseki].Moves[i].Y);
                }
                if (State == "training" || State == "learning")
                {
                    ResultPositionDemo = true;
                }
                if (State == "editor-overview")
                {
                    ResultPositionDemo = false;
                }
                BoardRenderer.ClearHint();
            }
        }

        public void ResetPosition()
        {
            Board.Clear();
            CurrentMove = 0;
            JosekiFinished = false;
            if (State == "training")
            {
                SetEmotion("liza-neutral");
                for (int i = 0; i < JosekiList.JosekiList[CurrentJoseki].StartingStones.Count; i++)
                {
                    Board.PlaceStone(JosekiList.JosekiList[CurrentJoseki].StartingStones[i].X,
                                        JosekiList.JosekiList[CurrentJoseki].StartingStones[i].Y,
                                        JosekiList.JosekiList[CurrentJoseki].StartingStones[i].Color);
                }
                if (JosekiList.JosekiList[CurrentJoseki].StartingStones.Count > 0)
                {
                    Board.CurrentColor = 1;
                }
                BoardEnabled = true;
                UpdateHint();
            }
            if (State == "learning")
            {
                SetEmotion("liza-neutral");
                if (JosekiList.JosekiList.Count > 0)
                {
                    for (int i = 0; i < JosekiList.JosekiList[CurrentJoseki].StartingStones.Count; i++)
                    {
                        Board.PlaceStone(JosekiList.JosekiList[CurrentJoseki].StartingStones[i].X,
                                            JosekiList.JosekiList[CurrentJoseki].StartingStones[i].Y,
                                            JosekiList.JosekiList[CurrentJoseki].StartingStones[i].Color);
                    }
                    if (JosekiList.JosekiList[CurrentJoseki].StartingStones.Count > 0)
                    {
                        Board.CurrentColor = 1;
                    }
                    BoardEnabled = true;
                    UpdateHint();
                }
            }
            if (State == "editor-starting-stones")
            {
                TempJoseki.StartingStones.Clear();
                Board.Clear();
            }
            if (State == "editor-writing-moves")
            {
                TempJoseki.Moves.Clear();
                for (int i = 0; i < TempJoseki.StartingStones.Count; i++)
                {
                    Board.PlaceStone(TempJoseki.StartingStones[i].X,
                                        TempJoseki.StartingStones[i].Y,
                                        TempJoseki.StartingStones[i].Color);
                }
                if (TempJoseki.StartingStones.Count > 0)
                {
                    Board.CurrentColor = 1;
                }
            }
        }

        void UpdateHint()
        {
            TopRightText = string.Empty;

            if (State == "training")
            {
                BoardRenderer.ClearHint();

                if (CurrentJoseki < JosekiList.JosekiList.Count)
                {
                    var minMove = Math.Max(0, CurrentMove - 2);
                    var maxMove = Math.Min(CurrentMove + 1, JosekiList.JosekiList[CurrentJoseki].Moves.Count);

                    var textBuilder = new StringBuilder();
                    for (var i = minMove; i < maxMove; ++i)
                    {
                        if (i != minMove) textBuilder.AppendLine();

                        textBuilder.AppendLine($"Ruch {i + 1}: {JosekiList.JosekiList[CurrentJoseki].Moves[i].Comment}");
                    }
                    TopRightText = textBuilder.ToString();
                }
            }
            else if (State == "learning")
            {
                if (CurrentMove < JosekiList.JosekiList[CurrentJoseki].Moves.Count)
                {
                    BoardRenderer.SetHint(JosekiList.JosekiList[CurrentJoseki].Moves[CurrentMove].Color,
                                            JosekiList.JosekiList[CurrentJoseki].Moves[CurrentMove].X,
                                            JosekiList.JosekiList[CurrentJoseki].Moves[CurrentMove].Y);
                }
                if (JosekiFinished)
                {
                    BoardRenderer.ClearHint();

                }
                else if (CurrentJoseki < JosekiList.JosekiList.Count)
                {
                    var minMove = Math.Max(0, CurrentMove - 2);
                    var maxMove = Math.Min(CurrentMove + 1, JosekiList.JosekiList[CurrentJoseki].Moves.Count);

                    var textBuilder = new StringBuilder();
                    for (var i = minMove; i < maxMove; ++i)
                    {
                        if (i != minMove) textBuilder.AppendLine();

                        textBuilder.AppendLine($"Ruch {i + 1}: {JosekiList.JosekiList[CurrentJoseki].Moves[i].Comment}");
                    }
                    TopRightText = textBuilder.ToString();
                }
            }
        }

        public void CheckLastMove(int pX, int pY)
        {
            int PreviousColor;
            if (Board.CurrentColor == 1)
            {
                PreviousColor = 0;
            }
            else
            {
                PreviousColor = 1;
            }

            if (JosekiList.JosekiList[CurrentJoseki].Moves[CurrentMove].X == pX &&
                JosekiList.JosekiList[CurrentJoseki].Moves[CurrentMove].Y == pY &&
                JosekiList.JosekiList[CurrentJoseki].Moves[CurrentMove].Color == PreviousColor)
            {
                LastMoveCorrect = true;
            }
            else
            {
                SetEmotion("liza-angry");
                LastMoveCorrect = false;
                BoardEnabled = false;
            }

            if (LastMoveCorrect && CurrentMove == JosekiList.JosekiList[CurrentJoseki].Moves.Count - 1)
            {
                BoardEnabled = false;
                JosekiFinished = true;
            }
        }

        public void MakeMove(int pX, int pY)
        {
            bool ClickedSlotEmpty = Board.isSlotEmpty(pX, pY);
            bool ClickedSlotKo = false;
            if (pX == Board.ko_X && pY == Board.ko_Y)
            {
                ClickedSlotKo = true;
            }

            if (State == "training")
            {
                if (ClickedSlotEmpty && !ClickedSlotKo)
                {
                    Board.MakeMove(pX, pY);
                    CheckLastMove(pX, pY);
                    CurrentMove++;
                    if (JosekiFinished)
                    {
                        SetEmotion("liza-happy");
                        RepeatJosekiTimer.Start();
                    }
                }
                else
                {
                    LastMoveCorrect = false;
                }
            }
            if (State == "learning")
            {
                if (ClickedSlotEmpty && !ClickedSlotKo)
                {
                    Board.MakeMove(pX, pY);
                    CheckLastMove(pX, pY);
                    CurrentMove++;
                    if (JosekiFinished)
                    {
                        SetEmotion("liza-happy");
                        RepeatJosekiTimer.Start();
                    }
                }
                else
                {
                    LastMoveCorrect = false;
                }
            }

            if (State == "editor-writing-moves")
            {
                if (ClickedSlotEmpty && !ClickedSlotKo)
                {
                    var comment = InputDialog.Prompt("Comment this move: (optional)", "Comment", inputType: InputDialog.InputType.Text)?.Trim();
                    comment = string.IsNullOrEmpty(comment) ? null : comment;

                    TempJoseki.Moves.Add(new BadukMove(Board.CurrentColor, Click_X, Click_Y, comment));
                    Board.MakeMove(Click_X, Click_Y);
                }
            }
        }

        public void MakeNextMove()
        {
            if (!JosekiFinished)
            {
                MakeMove(JosekiList.JosekiList[CurrentJoseki].Moves[CurrentMove].X,
                            JosekiList.JosekiList[CurrentJoseki].Moves[CurrentMove].Y);
                UpdateHint();
            }
        }

        public void UpdateInfoBlock()
        {
            TitleText = CurrentJoseki >= 0 && CurrentJoseki < JosekiList.JosekiList.Count ? JosekiList.JosekiList[CurrentJoseki].Title : null;

            TopLeftText = "";
            if (State == "started")
            {
                TopLeftText += "Press O key to open joseki collection";
                TopLeftText += "\nPress Tab to show/hide open joseki collection";
            }
            if (State == "training")
            {
                TopLeftText += "Training mode";
                TopLeftText += "\nCurrent joseki - " + (CurrentJoseki + 1).ToString();
                if (!ResultPositionDemo)
                {
                    TopLeftText += "\nRight click - reset position";
                }
            }
            if (State == "learning")
            {
                TopLeftText += "Learning mode";
                TopLeftText += "\nSpace - go to next joseki";
                TopLeftText += "\nRight click - reset position";
                TopLeftText += "\nCurrent joseki - " + (CurrentJoseki + 1).ToString();
            }

            if (State == "editor-overview")
            {
                TopLeftText += "Editor mode";
                TopLeftText += "\nOverview";
                TopLeftText += "\nCurrent joseki - " + (CurrentJoseki + 1).ToString();
                TopLeftText += "\nI - Insert new joseki";
                TopLeftText += "\nDelete - Delete current joseki";
            }

            if (State == "editor-starting-stones")
            {
                TopLeftText += "Editor mode";
                TopLeftText += "\nStarting stones";
                TopLeftText += "\nLeft click - place black stone";
                TopLeftText += "\nShit + Left click - place white stone";
                TopLeftText += "\nRight click - reset position";
                TopLeftText += "\nEnter - Go to moves";
            }

            if (State == "editor-writing-moves")
            {
                TopLeftText += "Editor mode";
                TopLeftText += "\nWriting moves";
                TopLeftText += "\nLeft click - make move";
                TopLeftText += "\nRight click - reset position";
                TopLeftText += "\nEnter - Save joseki";
            }

            if (State == "about")
            {
                TopLeftText =
@"A Go/Baduk/Weiqi joseki training software.
Oryginally developed by Nikolay Pridachin and opened for development by Crazy Samurais of Go Educational Center.

01.03.2022: Extended version developed by Lukasz Struzik to add Joseki names, move comments and GUI adjustments.
25.08.2021: GitHub repository open by Crazy Samurais of Go and source code shared.
30.07.2021: Original Source code shared by Nikolay Pridachin to Crazy Samurais of Go (https://szalenisamuraje.org/).
13.05.2020 Skillplay Joseki Master 1.0 developed by Nikolay Pridachin and shared at at this link https://yadi.sk/d/kFrxh2LsAl-fSQ.";
            }
        }

        public void RepeatJoseki()
        {
            if (State == "training" || State == "learning")
            {
                SetEmotion("liza-neutral");
                if (PlayerFirstMove)
                {
                    PlayerFirstMove = false;
                    ResetPosition();
                    MakeNextMove();
                }
                else
                {
                    PlayerFirstMove = true;
                    if (State == "training")
                    {
                        IncrementIndex();
                        ShowResultPosition();
                    }
                    if (State == "learning")
                    {
                        ResetPosition();
                        MakeNextMove();
                    }
                }
            }
            UpdateInfoBlock();
        }

        public void EditorGoNext()
        {
            if (State == "editor-overview")
            {
                IncrementIndex();
                ShowResultPosition();
                UpdateInfoBlock();
            }
        }

        public void EditorGoPrevious()
        {
            if (State == "editor-overview")
            {
                DecrementIndex();
                ShowResultPosition();
                UpdateInfoBlock();
            }
        }

        public void EditorInsertNewJoseki()
        {
            Board.Clear();
            BoardEnabled = true;
            TempJoseki = new BadukJoseki();

            var title = InputDialog.Prompt("Enter title: (optional)", "New Joseki", inputType: InputDialog.InputType.Text)?.Trim();
            if (!string.IsNullOrEmpty(title))
            {
                TempJoseki.Title = title.Trim();
            }

            SetState("editor-starting-stones");
        }

        public void EditorProcessToMoves()
        {
            SetState("editor-writing-moves");
            ResetPosition();
        }

        public void EditorSaveCurrentJoseki()
        {
            if (State == "editor-writing-moves")
            {
                JosekiList.JosekiList.Add(TempJoseki);
                if (CollectionFilenameWithPath.Length > 0)
                {
                    JosekiList.Export(CollectionFilenameWithPath);
                }
                CurrentJoseki = JosekiList.JosekiList.Count - 1;
                SetState("editor-overview");
            }
        }

        public void EditorDeleteCurrentJoseki()
        {
            if (State == "editor-overview")
            {
                if (JosekiList.JosekiList.Count > 0 &&
                    CurrentJoseki >= 0 &&
                    CurrentJoseki < JosekiList.JosekiList.Count &&
                    CollectionFilenameWithPath.Length > 0)
                {
                    JosekiList.JosekiList.RemoveAt(CurrentJoseki);
                    if (CurrentJoseki >= JosekiList.JosekiList.Count)
                    {
                        CurrentJoseki = JosekiList.JosekiList.Count - 1;
                        if (CurrentJoseki < 0)
                        {
                            CurrentJoseki = 0;
                        }
                    }
                }
                JosekiList.Export(CollectionFilenameWithPath);
                ShowResultPosition();
                UpdateInfoBlock();
            }
        }

        public void SetEmotion(string param)
        {
            if (param == "liza-neutral")
            {
                CharacterEmotion = "liza-neutral";
            }
            if (param == "liza-happy")
            {
                CharacterEmotion = "liza-happy";
            }
            if (param == "liza-angry")
            {
                CharacterEmotion = "liza-angry";
            }
        }

        public void SetResolution(int pX, int pY)
        {
            if (pX == 1920 && pY == 1080)
            {
                ClientWidth = pX;
                ClientHeight = pY;
                BoardRenderer.cell_size = 54;
                BoardRenderer.ScaleFactor = 1.2;
                BoardRenderer.SetResolution(pX, pY);
                CharacterWidth = (int)(400 * 1.2);
                CharacterHeight = (int)(600 * 1.2);
                FontSize = 20;
            }
            if (pX == 1600 && pY == 900)
            {
                ClientWidth = pX;
                ClientHeight = pY;
                BoardRenderer.cell_size = 45;
                BoardRenderer.ScaleFactor = 1.0;
                BoardRenderer.SetResolution(pX, pY);
                CharacterWidth = 400;
                CharacterHeight = 600;
                FontSize = 16;
            }
            if (pX == 1280 && pY == 720)
            {
                ClientWidth = pX;
                ClientHeight = pY;
                BoardRenderer.cell_size = 35;
                BoardRenderer.ScaleFactor = 0.8;
                BoardRenderer.SetResolution(pX, pY);
                CharacterWidth = (int)(400 * 0.8);
                CharacterHeight = (int)(600 * 0.8);
                FontSize = 12;
            }
            if (pX == 1024 && pY == 576)
            {
                ClientWidth = pX;
                ClientHeight = pY;
                BoardRenderer.cell_size = 27;
                BoardRenderer.ScaleFactor = 0.64;
                BoardRenderer.SetResolution(pX, pY);
                CharacterWidth = (int)(400 * 0.64);
                CharacterHeight = (int)(600 * 0.64);
                FontSize = 10;
            }
            if (pX == 854 && pY == 480)
            {
                ClientWidth = pX;
                ClientHeight = pY;
                BoardRenderer.cell_size = 22;
                BoardRenderer.ScaleFactor = 0.53;
                BoardRenderer.SetResolution(pX, pY);
                CharacterWidth = (int)(400 * 0.53);
                CharacterHeight = (int)(600 * 0.53);
                FontSize = 8;
            }
        }
    }
}
