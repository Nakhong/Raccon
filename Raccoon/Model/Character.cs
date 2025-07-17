using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Raccoon
{
    /// <summary>
    /// 캐릭터 관련 클래스
    /// </summary>
    public class Character
    {
        Bitmap leftSprites, rightSprites, ladderSprites, fallSpritess, jumpBit;
        private int x = 740; // 캐릭터 현재 X 좌표
        public int _X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }
        private float y = 534; // 캐릭터 현재 Y 좌표
        public float _Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }
        private bool jumpKey; //점프 키 눌림 여부
        public bool _JumpKey
        {
            get
            {
                return jumpKey;
            }
            set
            {
                jumpKey = value;
            }
        }
        private int dir = 0;// 캐릭터 방향 (0 : 왼쪽, 1 : 오른쪽)
        public int _Dir
        {
            get
            {
                return dir;
            }
            set
            {
                dir = value;
            }
        }
        private Rectangle chRect; // 캐릭터 충돌 영역
        public Rectangle _ChRect
        {
            get
            {
                return chRect;
            }
            set
            {
                chRect = value;
            }
        }

        int cState = 0; // 캐릭터 현재 상태(1 : 왼쪽 2 : 오른쪽 3: 사다리 위로 4: 사다리 아래로)
        public int _State
        {
            get { return cState; }
            set { cState = value; }
        }
        int jump = 0; // 점프 상태(0 : 착지 1 : 점프 중)
        public int _Jump
        {
            get { return jump; }
            set { jump = value; }
        }

        private bool xStop; // X축 이동 멈출지
        public bool _XStop
        {
            get
            {
                return xStop;
            }
            set
            {
                xStop = value;
            }
        }
        private bool jumpStop; // 점프 막을지
        public bool _JumpStop
        {
            get
            {
                return jumpStop;
            }
            set
            {
                jumpStop = value;
            }
        }
        int time = 0; // 타이머
        int fallTime = 0; // 낙하 타이머
        int lIndex = 0; // 왼쪽 이동 인덱스
        int rIndex = 0; // 오른쪽 이동 인덱스
        int upIndex = 0; // 사다리 오르기 인덱스
        int fallIndex = 0; // 낙하 인덱스
        int width, height; // 캐릭터 가로 세로 값
        float baseY = 534; // 현재 서 있는 지면 Y 좌표
        int xrLimit = 752; // 오른쪽 이동 제한 X 좌표
        int xlLimit = 50; // 왼쪽 이동 제한 Y 좌표
        int xjump = 2; // 점프 시 X축 속도
        float g = 0.175f; // 중력 가속도
        float speed = 2.5f; // 점프 스피드
        int jumpTime = 0; // 점프 시간
        bool lAni, rAni, mode, mode1, mode2, mode3, longJump, fall, fall1, playAni, ladderUp;
        // lAni: 왼쪽 애니메이션 활성화
        // rAni: 오른쪽 애니메이션 활성화
        // mode: Y 좌표 도달 시(사다리)
        // mode1: 벽에 부딪혔을 때 점프 동작 제어(벽에 붙지 않게)
        // mode2: 점프 끝났을 때 점프 관련 변수 초기화
        // mode3: 롱점프 중인지
        // longJump: 롱점프 활성화 여부(방향키 + 스페이스바)
        // fall: 장애물 충돌로 인한 낙하
        // fall1: 적과의 충돌로 인한 낙하
        // playAni: 낙하 애니메이션 재생
        // ladderUp: 사다리 올라가는 중인지
        Rectangle[] ladder = new Rectangle[5]; // 사다리 영역 저장
        Rectangle[] rectangles = new Rectangle[7]; // 이동 스프라이트의 잘라낼 영역 저장
        Rectangle[] ladders = new Rectangle[3]; // 사다리 오르기 스프라이트의 잘라낼 영역 저장

        /// <summary>
        /// 캐릭터 생성자
        /// </summary>
        public Character()
        {
            arrBit();
            createLadder();
        }
        /// <summary>
        /// 스프라이트 시트에 애니메이션 프레임을 잘라낼 영역 설정
        /// </summary>
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
        /// <summary>
        /// 사다리 영역 생성
        /// </summary>
        private void createLadder()  //사다리 범위 생성
        {
            ladder[0] = new Rectangle(410, 476, 24, 96);
            ladder[1] = new Rectangle(554, 380, 24, 98);
            ladder[2] = new Rectangle(194, 283, 23, 98);
            ladder[3] = new Rectangle(387, 189, 22, 96);
            ladder[4] = new Rectangle(650, 189, 22, 96);
        }
        /// <summary>
        /// 캐릭터 애니메이션 메서드
        /// </summary>
        /// <param name="gameover"></param>
        /// <param name="gameover1"></param>
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
        /// <summary>
        /// 초기화 메서드
        /// </summary>
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
        /// <summary>
        /// 캐릭터 점프 및 롱점프 메서드
        /// </summary>
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
        /// <summary>
        /// 그리기 메서드
        /// </summary>
        /// <param name="e"></param>
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
        /// <summary>
        /// 캐릭터 현재 상태에 따라 상태 갱신 메서드
        /// </summary>
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
        /// <summary>
        /// 사다리 위/아래 도달 시 지면 설정 메서드
        /// </summary>
        public void tempY()
        {
            if (!mode)
            {
                baseY = y;  //사다리를 다 올라왔을때 y좌표를 수정
                mode = true;
            }
        }
        /// <summary>
        /// 도토리 발사 위치 좌표
        /// </summary>
        /// <returns></returns>
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
