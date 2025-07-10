using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Raccoon
{
    public class Character
    {
        Bitmap leftSprites, rightSprites, ladderSprites, fallSpritess, jumpBit;
        public int x = 740;
        public float y = 534;
        int cState = 0;
        public bool jumpKey;
        int time = 0;
        int fallTime = 0;
        int lIndex = 0;
        int rIndex = 0;
        int upIndex = 0;
        int fallIndex = 0;
        public int dir = 0;
        int width, height;
        float baseY = 534;
        int jump = 0;
        int xrLimit = 752;
        int xlLimit = 50;
        int xjump = 2;
        float g = 0.175f;
        float speed = 2.5f;
        int jumpTime = 0;
        public bool xStop, lAni, rAni, mode, mode1, mode2, mode3, jumpStop, longJump, fall, fall1, playAni, ladderUp;
        public Rectangle chRect;
        Rectangle[] ladder = new Rectangle[5];
        Rectangle[] rectangles = new Rectangle[7];
        Rectangle[] ladders = new Rectangle[3];

        public int State
        {
            get { return cState; }
            set { cState = value; }
        }

        public int Jump
        {
            get { return jump; }
            set { jump = value; }
        }

        public Character()
        {
            arrBit();
            createLadder();
        }

        private void arrBit()
        {
            leftSprites = Properties.Resources.CharterLeftSprite;
            rightSprites = Properties.Resources.CharterRightSprite;
            ladderSprites = Properties.Resources.LadderSprite;
            fallSpritess = Properties.Resources.fallSprite;
            width = leftSprites.Width / 7;
            height = leftSprites.Height;

            for (int i = 0; i < 7; i++)  // 캐릭터 이동 스프라이트를 자를영역 생성
                rectangles[i] = new Rectangle(width * i, 0, width, height);
            for (int i = 0; i < 3; i++)  //캐릭터 사다리올라가는 스프라이트를 자를영역 생성
                ladders[i] = new Rectangle(width * i, 0, width, height);
        }

        private void createLadder()  //사다리 범위 생성
        {
            ladder[0] = new Rectangle(410, 476, 24, 96);
            ladder[1] = new Rectangle(554, 380, 24, 98);
            ladder[2] = new Rectangle(194, 283, 23, 98);
            ladder[3] = new Rectangle(387, 189, 22, 96);
            ladder[4] = new Rectangle(650, 189, 22, 96);
        }

        public void ani(bool gameover, bool gameover1)  //캐릭터 좌우 이동 애니, 사다리 올라가는 애니, 게임오버시 추락하는 애니
        {
            fall = gameover;        //장애물과 충돌했을시 gameover이 true
            fall1 = gameover1;      //적과 충돌했을시 gameover1이 true
            time++;
            if (time > 2 && lIndex < 7 && lAni)
            {
                dir = 0;
                lIndex++;
                time = 0;
            }

            if (lIndex == 7 || !lAni)
                lIndex = 0;

            if (time > 2 && rIndex < 7 && rAni)
            {
                dir = 1;
                rIndex++;
                time = 0;
            }

            if (rIndex == 7 || !rAni)
                rIndex = 0;

            if (time > 10 && upIndex < 4 && xStop && (cState == 3 || cState == 4))
            {
                upIndex++;
                time = 0;
            }
            if (upIndex == 3)
                upIndex = 0;

            if ((fall || fall1) && y < 534)
            {
                fallTime++;
                playAni = true;
                xStop = true;
                jump = 0;
                y += 1.7f;
            }
            if (y > 534)            //바닥끝으로 추락하면 머리가 땅으로 향해있는 모습으로 설정 fallindex4가 해당 그림임
                fallIndex = 4;
            if ((fall || fall1) && y == baseY)
            {
                fallIndex = 4;
                playAni = true;
                xStop = true;
                jump = 0;
            }

            if ((fall || fall1) && fallTime > 6 && fallIndex < 7)   //추락할때 보여주는 이미지 일정시간마다 바꿔줌
            {
                fallIndex++;
                fallTime = 0;
            }

            if (fallIndex == 7)
                fallIndex = 0;

            jumping();
        }

        public void reSet()     // 다시시작 버튼 클릭시 초기값으로 다시설정하는 함수
        {
            x = 740;
            y = 534;
            jump = 0;
            baseY = 534;
            jumpTime = 0;
            xStop = false;
            mode3 = false;
            jumpKey = false;
            longJump = false;
            mode2 = true;
            playAni = false;
            dir = 0;
        }

        private void jumping()
        {
            if (jumpKey && (cState == 1 || cState == 2) && !mode3)   //좌,우 화살표키와 스페이스 를 같이누르면 해당변수 true로 바꿈
                longJump = true;

            if (jump == 1 && !xStop)  // 스페이스를 누르면 jump가 1이됨
            {
                if (dir == 0)     // dir이 0이면 캐릭터가 왼쪽을 향한상태
                {
                    if (!longJump)   //롱점프아님
                    {
                        g = 0.175f;
                        speed = 2.5f;
                    }
                    if (longJump)  //롱점프시에
                    {
                        g = 0.039375f;
                        speed = 1.2f;
                    }

                    mode3 = true;
                    x -= xjump;
                    y -= speed - g * jumpTime;   //중력 가속도
                    jumpTime++;
                    if (speed - g * jumpTime > 0)
                        jumpBit = Properties.Resources.leftJump;
                    else
                        jumpBit = Properties.Resources.leftJump1;
                    mode2 = false;
                    if (y > baseY && !mode2)   // 점프가 끝났을때
                    {
                        mode3 = false;
                        jumpKey = false;
                        longJump = false;
                        g = 0.175f;
                        speed = 2.5f;
                        y = baseY;
                        jump = 0;
                        jumpTime = 0;
                        mode2 = true;    // mode2를 true바꿔주면서 해당 if문이 한번만 실행되게 해줌
                    }
                }
                if (dir == 1)
                {
                    if (!longJump)  //위와 같음
                    {
                        g = 0.175f;
                        speed = 2.5f;
                    }
                    if (longJump)   //위와 같음
                    {
                        g = 0.039375f;
                        speed = 1.12f;
                    }
                    mode3 = true;
                    x += xjump;
                    y -= speed - g * jumpTime;
                    jumpTime++;
                    if (speed - g * jumpTime > 0)
                        jumpBit = Properties.Resources.rightJump;
                    else
                        jumpBit = Properties.Resources.rightJump1;
                    mode2 = false;
                    if (y > baseY && !mode2)   //위와 같음
                    {
                        mode3 = false;
                        jumpKey = false;
                        longJump = false;
                        g = 0.175f;
                        speed = 2.5f;
                        y = baseY;
                        jump = 0;
                        jumpTime = 0;
                        mode2 = true;    //위와 같음
                    }
                }
            }

            if ((x < 51 || x > xrLimit) && y < baseY)  //점프했을시 양쪽 벽끝에 충돌했을때 캐릭터를 바로 지면으로 떨어뜨림
            {
                y += 2;
                jump = 0;
                if (y > baseY)    //지면에 충돌했을시 점프에 필요했던 변수들을 초기화시킴
                {
                    y = baseY;
                    jumpKey = false;
                    g = 0.175f;
                    speed = 2.5f;
                    mode3 = false;
                    jumpTime = 0;
                    longJump = false;
                }
            }

            if (xrLimit <= x || xlLimit >= x) //양쪽 벽끝에 있을때는 점프를 못하게막음
                jumpStop = true;
            else
                jumpStop = false;

            if (y <= 438)         // 2층으로가게되면 오른쪽 벽끝의 좌표를 바꿔줌
                xrLimit = 684;
            else
                xrLimit = 752;
            if (jumpStop && baseY != y)    //벽끝에 있고 지면과 닿지 않았을때(점프상태일때)
                mode1 = true;
            else
                mode1 = false;
        }

        public void draw(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (!playAni)                        //추락하는 상태가 아닐때
            {
                if (lAni && !xStop && jump == 0)
                    g.DrawImage(leftSprites, new Rectangle(x, (int)y, 40, 40), rectangles[lIndex], GraphicsUnit.Pixel);
                else if (rAni && !xStop && jump == 0)
                    g.DrawImage(rightSprites, new Rectangle(x, (int)y, 40, 40), rectangles[rIndex], GraphicsUnit.Pixel);
                if (!lAni && dir == 0 && !xStop && jump == 0)
                    g.DrawImage(leftSprites, new Rectangle(x, (int)y, 40, 40), rectangles[0], GraphicsUnit.Pixel);
                if (!rAni && dir == 1 && !xStop && jump == 0)
                    g.DrawImage(rightSprites, new Rectangle(x, (int)y, 40, 40), rectangles[0], GraphicsUnit.Pixel);
                if (xStop)
                    g.DrawImage(ladderSprites, new Rectangle(x, (int)y, 40, 40), ladders[upIndex], GraphicsUnit.Pixel);
                if (jump == 1)
                    g.DrawImage(jumpBit, new Rectangle(x, (int)y, 40, 40), new Rectangle(0, 0, 40, 40), GraphicsUnit.Pixel);
            }
            if ((fall || fall1))   //추락하는 상태일때
                g.DrawImage(fallSpritess, new Rectangle(x, (int)y, 40, 40), rectangles[fallIndex], GraphicsUnit.Pixel);

        }

        public void move()
        {
            if (cState == 1 && x > 51 && !xStop && !mode1 && !mode3)  // 좌로이동
            {
                x -= 2;
                lAni = true;   //캐릭터 이동중에 보여줄 애니메이션 조건설정
            }
            else
                lAni = false; 
            if (cState == 2 && x < xrLimit && !xStop && !mode1 && !mode3) //우로이동
            {
                x += 2;
                rAni = true;   //캐릭터 이동중에 보여줄 애니메이션 조건설정
            }
            else
                rAni = false;

            for (int i = 0; i < 5; i++)
            {
                if (chRect.IntersectsWith(ladder[i]) && cState == 3 && !jumpKey) //캐릭터가 사다리영역에 들어왔을때, 점프상태가 아닐때
                {
                    if (ladder[i].Top + 2 < y + 40 && jump == 0)  //들어온 해당 사다리의 위쪽까지만 이동
                    {

                        y -= 2;
                        xStop = true;     //좌우 이동을막음
                        mode = false;   
                    }
                    else
                    {
                        xStop = false;
                        tempY();          //사다리 다 올라왔을때 지면의 y좌표를 수정해주는 함수
                    }

                }
                if (chRect.IntersectsWith(ladder[i]) && cState == 4 && !jumpKey) //위와 같음
                {
                    if (ladder[i].Bottom > y + 40 && jump == 0)   //위와 같음
                    {
                        y += 2;
                        xStop = true;  //위와 같음
                        mode = false;
                    }
                    else
                    {
                        xStop = false;
                        tempY();    //위와 같음
                    }
                }
            }

            chRect = new Rectangle(x + 10, (int)y, 15, 40); //캐릭터의 충돌범위
        }

        public void tempY()
        {
            if (!mode)
            {
                baseY = y;  //사다리를 다 올라왔을때 y좌표를 수정
                mode = true;
            }
        }

        public Point GetAttackPosition()
        {
            // 도토리가 캐릭터의 중앙에서 발사되도록
            if (dir == 0) // 왼쪽
            {
                return new Point(chRect.X, chRect.Y + chRect.Height / 2 - 10); // 도토리 Y 위치 조정
            }
            else // 오른쪽
            {
                return new Point(chRect.Right - 20, chRect.Y + chRect.Height / 2 - 10); // 도토리 Y 위치 조정, 캐릭터 우측에서 시작
            }
        }

    }
}
