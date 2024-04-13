using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    public class GameState
    {
        public int Rows { get; } 
        public int Columns { get; }
        public GridValue[,] Grid { get; }
        public Direction Dir { get; private set; }
        public int Score { get; private set;}
        public bool GameOver { get; private set;}

        private readonly LinkedList<Position> snakePositions = new LinkedList<Position>();
        private readonly Random random = new Random();

        public GameState(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            Grid = new GridValue[rows, columns]; //tömb, alap esetben mindenhol empty
            Dir = Direction.Right;

            AddSnake();
            AddFood();
        }

        private void AddSnake()
        {
            int r = Rows / 2;           //a középső sorban, 123 oszlopokban lesz a snake
            for(int i=1;i <= 3; i++)
            {
                Grid[r, i] = GridValue.Snake;
                snakePositions.AddFirst(new Position(r, i));
            }
        }
        private IEnumerable<Position> EmptyPositions()  //üres pozik, hoy hová kerülhet étel
        {
            for(int i = 0; i < Rows; i++)
            {
                for(int j = 0;j < Columns; j++)
                {
                    if (Grid[i,j] == GridValue.Empty)
                    {
                        yield return new Position(i, j);
                    }
                }
            }
        }

        private void AddFood()
        {
            List<Position> empty = new List<Position>(EmptyPositions());

            if(empty.Count == 0)
            {
                return;
            }

            Position pos = empty[random.Next(empty.Count)];
            Grid[pos.Row,pos.Column] = GridValue.Food;

        }

        public Position HeadPosition()
        {
            return snakePositions.First.Value;
        }
        public Position TailPosition()
        {
            return snakePositions.Last.Value;
        }
        public IEnumerable<Position> SnakePositions()
        {
            return snakePositions;
        }
        private void AddHead(Position pos)
        {
            snakePositions.AddFirst(pos);
            Grid[pos.Row, pos.Column] = GridValue.Snake;
        }
        private void RemoveTail()
        {
            Position tail = snakePositions.Last.Value;
            Grid[tail.Row, tail.Column] = GridValue.Empty;
            snakePositions.RemoveLast();
        }
        public void ChangeDirection(Direction direction)
        {
            Dir = direction;
        }
        private bool OutsideGrid(Position pos)      //kimegy -e a keretből
        {
            return pos.Row < 0 || pos.Row >= Rows || pos.Column < 0 || pos.Column >= Columns;
        }
        private GridValue WillHit(Position newHeadPos)
        {
            if (OutsideGrid(newHeadPos))
            {
                return GridValue.Outside;
            }
            if(newHeadPos == TailPosition())
            {
                return GridValue.Empty;
            }
            return Grid[newHeadPos.Row,newHeadPos.Column];
        }
        public void Move()
        {
            Position newHeadPos = HeadPosition().Translate(Dir);
            GridValue hit = WillHit(newHeadPos);
            if(hit == GridValue.Outside || hit == GridValue.Snake)
            {
                GameOver = true;
            }else if(hit == GridValue.Empty)
            {
                RemoveTail();
                AddHead(newHeadPos);
            }else if(hit == GridValue.Food)
            {
                AddHead(newHeadPos);
                Score++;
                AddFood();
            }
        }
    }
}
