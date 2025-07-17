using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Drawing.Text;

namespace Raccoon
{
    /// <summary>
    /// 장애물 및 아이템 관련 클래스
    /// </summary>
    public class Obstruction
    {
        private bool eat, scoreAni;
        private bool gameover;
        public bool _Gameover // 게임 끝인지
        {
            get
            {
                return gameover;
            }
            set
            {
                gameover = value;
            }
        }
        private int count = 0; // 아이템 먹었을 때 상승 시킴. 아이템그려주기 위해서
        public int _Count
        {
            get
            {
                return count;
            }
            set
            {
                count = value;
            }
        }
        private int score = 0; // 현재 점수
        public int _Score
        {
            get
            {
                return score;
            }
            set
            {
                score = value;
            }
        }
        Rectangle[] hole = new Rectangle[6]; // 맵에서 낭떠러지 영역
        Rectangle[] gimlets = new Rectangle[13]; // 장애물 영역
        Rectangle[] gimletsCol = new Rectangle[13]; //장애물 충돌 영역
        Rectangle[] itemApple = new Rectangle[6]; // 사과 아이템 영역
        Rectangle[] itemBanana = new Rectangle[4]; // 바나나 아이템 영역
        Rectangle[] rectangles = new Rectangle[10]; // 획득한 아이템 그려주는 영역
        Rectangle chBottom; //충돌 검사 영역
        Rectangle delete= new Rectangle(0, 0, 0, 0); // 영역 삭제
        Rectangle scorePo;  // 점수 표시될 위치
        Bitmap gimlet, apple, banana; // 이미지
        Bitmap[] bitmap = new Bitmap[10]; // 이미지 저장
        Random rand = new Random(); // 랜덤 아이템
        int[] baseY = new int[13]; // 기본 y좌표
        int[] min = new int[13]; // 최소 좌표
        int[] max = new int[13]; // 최대 좌표
        int[] minApple = new int[6]; // 사과 최소 좌표
        int[] maxApple = new int[6]; // 사과 최대 좌표
        int[] minBanana = new int[4]; // 바나나 최소 좌표
        int[] maxBanana = new int[4];   // 바나나 최대 좌표
        int[] randX = new int[13]; // 랜덤 x 좌표
        int[] randApple = new int[6]; // 사과 랜덤 좌표
        int[] randBanana = new int[4]; // 바나나 랜덤 좌표
        int yCount = 0; // 모든 y좌표 카운트
        int y = 434; // y 좌표
        int scoreData; // 획득한 아이템의 점수 값
        int time = 0; // 시간 카운터
        Font font; // 폰트
        PrivateFontCollection privateFonts; // 사용자 정의 글꼴 로드 및 관리

        /// <summary>
        /// Object 생성자
        /// </summary>
        public Obstruction()
        {
            createHole();
            createGimlet();
            crateItem();
            itemUiList();
            createFont();
        }
        /// <summary>
        /// 초기화
        /// </summary>
        public void reSet()
        {
            yCount = 0;
            count = 0;
            y = 434;
            score = 0;
            gameover = false;
            createGimlet();
            crateItem();
            itemUiList();          
        }
        /// <summary>
        /// 폰트 설정
        /// </summary>
        void createFont()
        {
            privateFonts = new PrivateFontCollection();
            privateFonts.AddFontFile("neoletters.ttf");
            font = new Font(privateFonts.Families[0], 10f, FontStyle.Regular);
        }
        /// <summary>
        /// 획득한 아이템을 보여줄 영역
        /// </summary>
        void itemUiList()
        {
            for (int i = 0; i < 10; i++)
            {
                rectangles[i] = new Rectangle(775, y, 40, 40);
                y -= 30;
            }
        }
        /// <summary>
        /// 장애물의 배치 범위
        /// </summary>
        void createGimlet()
        {
            gimlet = Properties.Resources.gimlet;            
            baseY[0] = 549;
            baseY[1] = 453;
            baseY[2] = 358;
            baseY[3] = 262;
            baseY[4] = 166;
            min[0] = 600;
            max[0] = 660;
            min[1] = 450;
            max[1] = 550;
            min[2] = 70;
            max[2] = 350;
            min[3] = 70;
            max[3] = 200;
            min[4] = 295;         //장애물(송곳)의 배치 범위를 랜덤하게 가질 최소, 최대값
            max[4] = 376;
            min[5] = 623;
            max[5] = 671;
            min[6] = 613;
            max[6] = 675;
            min[7] = 262;
            max[7] = 350;
            min[8] = 80;
            max[8] = 145;    
            min[9] = 140;
            max[9] = 160;
            min[10] = 430;
            max[10] = 592;
            min[11] = 507;
            max[11] = 589;
            min[12] = 289;
            max[12] = 355;

            for (int i = 0; i < 13; i++)
            {
                if (i != 0 && i % 3 == 0 && i < 10)
                    yCount++;
                if (i == 11)
                    yCount++;
                randX[i] = rand.Next(min[i], max[i]);  // 송곳의 x좌표를 랜덤하게설정
                gimlets[i] = new Rectangle(randX[i], baseY[yCount], 30, 30); //송곳을 그릴영역
                gimletsCol[i] = new Rectangle(randX[i] + 8, baseY[yCount] + 11, 10, 10); //송곳의 충돌범위
            }          

        }
        /// <summary>
        /// 아이템 배치 범위
        /// </summary>
        void crateItem()
        {
            minApple[0] = 560;
            maxApple[0] = 580;
            minApple[1] = 50;
            maxApple[1] = 51;
            minApple[2] = 595;
            maxApple[2] = 600;
            minApple[3] = 588;
            maxApple[3] = 610;
            minApple[4] = 229;
            maxApple[4] = 240;    //아이템의 배치 범위를 랜덤하게 가질 최소, 최대값
            minApple[5] = 600;
            maxApple[5] = 690;
            minBanana[0] = 380;
            maxBanana[0] = 390;
            minBanana[1] = 500;
            maxBanana[1] = 512;
            minBanana[2] = 143;
            maxBanana[2] = 190;
            minBanana[3] = 54;
            maxBanana[3] = 76;            

            yCount = 0;
            apple = Properties.Resources.apple;
            banana = Properties.Resources.banana;
            for (int i = 0; i < 6; i++)   //사과 아이템
            {
                if (i == 1)
                    yCount++;
                if (i == 3)
                    yCount++;
                if(i == 5)
                    yCount++;
                randApple[i] = rand.Next(minApple[i], maxApple[i]); //사과의 랜덤x좌표
                itemApple[i] = new Rectangle(randApple[i], baseY[yCount]+5, 20, 20);                
            }
            yCount = 0;
            for(int i = 0; i < 4; i++)  // 바나나 아이템
            {
                if (i == 1)
                    yCount++;
                if(i == 2)
                    yCount+=3;
                randBanana[i] = rand.Next(minBanana[i], maxBanana[i]);  //바나나의 랜덤x좌표
                itemBanana[i] = new Rectangle(randBanana[i], baseY[yCount] + 5, 20, 20);                
            }
        }
        /// <summary>
        /// 맵에 낭떠러지 영역 생성
        /// </summary>
        void createHole()
        {
            hole[0] = new Rectangle(249, 476, 12, 21);
            hole[1] = new Rectangle(512, 380, 12, 21);
            hole[2] = new Rectangle(247, 282, 36, 21);
            hole[3] = new Rectangle(103, 187, 14, 21);
            hole[4] = new Rectangle(223, 187, 14, 21);
            hole[5] = new Rectangle(439, 187, 14, 21);
        }

        /// <summary>
        /// Object들 그려주는 이벤트
        /// </summary>
        /// <param name="e"></param>
        public void draw(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            /*for (int i = 0; i < 6; i++)
                g.DrawRectangle(new Pen(Brushes.Red), hole[i]);*/
            for (int i = 0; i < 13; i++) // 송곳 장애물
            {
                g.DrawImage(gimlet, gimlets[i], new Rectangle(0, 0, 20, 20), GraphicsUnit.Pixel);
                //g.DrawRectangle(new Pen(Brushes.Red), gimletsCol[i]);
            }
            for (int i = 0; i < 6; i++) // 사과 아이템
            {                
                g.DrawImage(apple, itemApple[i], new Rectangle(0, 0, 20, 20), GraphicsUnit.Pixel);
                //g.DrawRectangle(new Pen(Brushes.Red), itemApple[i]);
            }
            for(int i = 0; i < 4; i++) //바나나 아이템
            {
                g.DrawImage(banana, itemBanana[i], new Rectangle(0, 0, 20, 20), GraphicsUnit.Pixel);
                //g.DrawRectangle(new Pen(Brushes.Red), itemBanana[i]);
            }
            for (int i = 0; i < count; i++) // 우측의 먹은 아이템표시 아이콘
                g.DrawImage(bitmap[i], rectangles[i], new Rectangle(0, 0, 30, 30), GraphicsUnit.Pixel);
            
            if(scoreAni)  // 획득한 아이템의 점수를 그려줌
            g.DrawString(scoreData.ToString(), font, Brushes.White, scorePo.Left, scorePo.Top);

            //g.DrawRectangle(new Pen(Brushes.Red), chBottom);

        }
        /// <summary>
        /// 충돌에 대한 이벤트 계산
        /// </summary>
        /// <param name="chRect"></param>
        public void collision(Rectangle chRect)
        {
            chBottom = new Rectangle(chRect.Left, chRect.Top , 20, 30);
            for (int i = 0; i < 6; i++) // 낙하 게임오버
            {
                if (chRect.IntersectsWith(hole[i]))
                    gameover = true;
            }
            for (int i = 0; i < 13; i++) // 송곳 충돌 게임오버
            {
                if (chBottom.IntersectsWith(gimletsCol[i]))
                    gameover = true;
            }

            for(int i = 0; i < 6; i++) // 사과 아이템 충돌
            {
                if (chBottom.IntersectsWith(itemApple[i]))
                {
                    scorePo = itemApple[i];
                    scoreAni = true;
                    itemApple[i] = delete;  //먹은아이템의 Rectangle을 0으로 설정함으로써 없애줌
                    score += 300;                    
                    bitmap[count] = Properties.Resources.apple; // 획득한 아이템의 그림을 bitmap배열에 넣어서 우측에 표시해줌
                    count++;
                    scoreData = 300;    //사과의 점수
                }              
            }
            
            for(int i = 0; i <4; i++) // 바나나 아이템 충돌
            {
                if (chBottom.IntersectsWith(itemBanana[i]))
                {
                    scorePo = itemBanana[i];
                    scoreAni = true;
                    itemBanana[i] = delete;  //위와 같음
                    score += 600;                    
                    bitmap[count] = Properties.Resources.banana;  //위와 같음
                    count++;
                    scoreData = 600;     //바나나의 점수
                }                
            }
            if(scoreAni)   //먹게된 아이템의 점수를 time이 50이 될때동안만 보여줌
            {
                time++;
                if (time > 50)
                {
                    scoreAni = false;
                    time = 0;
                }
            }            
        }
        
    }
}