using Raccoon.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Raccoon
{
    /// <summary>
    /// 적 몬스터 관련 클래스
    /// </summary>
    public class Enemy
    {
        Random rand = new Random(); // 랜덤 값 생성
        int[] temp = new int[3]; // 적의 Y 좌표를 무작위로 선택하기 위한 임시 배열 (baseY의 인덱스 저장)
        int[] baseY = new int[3]; // 일반 적 몬스터가 배치될 수 있는 고정된 Y 좌표들
        int x = 51; // 적 몬스터들의 X 좌표
        int x1; // 비밀 항아리에서 나오는 적의 현재 X 좌표
        Bitmap[] bitmap = new Bitmap[4]; // 적 몬스터 이미지들
        Bitmap scret; // 비밀 항아리 아이템 이미지
        private Rectangle[] enemyRect = new Rectangle[3]; // 적 몬스터 3마리의 충돌 영역
        public Rectangle[] _EnemyRect
        {
            get
            {
                return enemyRect;
            }
            set
            {
                enemyRect = value;
            }
        }
        private int score = 0; // 총 점수
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
        private bool mode; // 비밀 항아리에서 나오는 적 몬스터 활성화
        public bool _Mode
        {
            get
            {
                return mode;
            }
            set
            {
                mode = value;
            }
        }
        private bool twinkle; // 비밀 항아리에서 나오는 적 몬스터 깜빡임
        public bool _Twinkle
        {
            get
            {
                return twinkle;
            }
            set
            {
                twinkle = value;
            }
        }
        private bool gameOver; // 게임 오버 상태
        public bool _GameOver
        {
            get
            {
                return gameOver;
            }
            set
            {
                gameOver = value;
            }
        }
        private Rectangle scretColRect; // 비밀 항아리에서 나오는 적 몬스터의 충돌 영역
        public Rectangle _ScretColRect
        {
            get
            {
                return scretColRect;
            }
            set
            {
                scretColRect = value;
            }
        }

        Rectangle[] scretRect = new Rectangle[4]; // 비밀 항아리 아이템 4개의 위치 및 충돌 영역
        Rectangle deletRect = new Rectangle(0, 0, 0, 0); // 영역 삭제
        Rectangle scretPo; // 비밀 항아리에서 나온 적 초기 생성 위치
        Rectangle scorePo; // 점수 표시될 위치

        bool right, left, right1, left1, mode1, respone, scoreAni;
        // right: 적이 오른쪽으로 이동하는지
        // left: 적이 왼쪽으로 이동하는지
        // right1: 비밀 항아리 적이 오른쪽으로 이동하는지
        // left1: 비밀 항아리 적이 왼쪽으로 이동하는지
        // mode1: 비밀 항아리 적의 초기 이동 방향 (false: 오른쪽으로 시작, true: 왼쪽으로 시작) 및 이동 상태 전환
        // respone: 비밀 항아리 적의 무적 시간 (true: 공격 가능, false: 무적)
        // scoreAni: 점수 표시 애니메이션 활성화

        int time = 0; // 적 애니메이션/이동 타이머
        int time1 = 0; // 비밀 항아리 적 몬스터 애니메이션 타이머
        int time2 = 0; // 비밀 항아리 적 몬스터 깜빡임 제어 타이머
        int time3 = 0; // 비밀 항아리 적 몬스터 무적 시간 제어 타이머
        int time4 = 0; // 점수 애니메이션 지속 시간 타이머
        int scretRand; // 비밀 항아리에서 나올 아이템/몬스터 종류를 결정하는 랜덤 값

        Font font; // 폰트
        PrivateFontCollection privateFonts; // 사용자 정의 글꼴 로드 및 관리

        private int[] enemyHP = new int[4]; // 적들의 체력을 저장하는 배열
        private int INITIAL_ENEMY_HP = 1; // 적 몬스터 초기 기본 체력 값
        private List<Items> activeItems = new List<Items>(); // 현재 화면에 활성화되어 있는 아이템들을 담는 리스트

        /// <summary>
        /// 적 생성자 메서드
        /// </summary>
        public Enemy()
        {
            yRandom();
            createScret();
            createFont();
            InitializeEnemyHP();
        }

        /// <summary>
        /// 적 체력 초기화 메서드
        /// </summary>
        void InitializeEnemyHP()
        {
            for (int i = 0; i < enemyHP.Length; i++) // enemyHP 배열의 모든 요소 초기화
            {
                enemyHP[i] = INITIAL_ENEMY_HP;
            }
        }
        /// <summary>
        /// 적 초기화 메서드
        /// </summary>
        public void reSet()
        {
            yRandom();
            createScret();
            x = 51;
            time2 = 0;
            time3 = 0;
            score = 0;
            respone = false;
            mode = false;
            gameOver = false;
            InitializeEnemyHP(); //체력 추가
        }
        /// <summary>
        /// 폰트 생성 메서드
        /// </summary>
        void createFont()
        {
            privateFonts = new PrivateFontCollection();
            privateFonts.AddFontFile("neoletters.ttf");
            font = new Font(privateFonts.Families[0], 10f, FontStyle.Regular);
        }
        /// <summary>
        /// 비밀 항아리 이미지 및 초기 위치, 충돌 영역 설정 메서드
        /// </summary>
        void createScret()
        {
            scret = Properties.Resources.scretItem;
            scretRect[0] = new Rectangle(450, 445, 35, 35);
            scretRect[1] = new Rectangle(370, 349, 35, 35);
            scretRect[2] = new Rectangle(85, 253, 35, 35);
            scretRect[3] = new Rectangle(680, 157, 35, 35);
        }
        /// <summary>
        /// 적 이미지 로드 및 적 y 좌료 무작위 선택 메서드
        /// </summary>
        void yRandom()
        {
            bitmap[0] = Properties.Resources.rightEnemy;
            bitmap[1] = Properties.Resources.rightEnemy1;
            bitmap[2] = Properties.Resources.rightEnemy1;
            bitmap[3] = Properties.Resources.rightEnemy;
            baseY[0] = 443;
            baseY[1] = 348;
            baseY[2] = 156;
            for (int i = 0; i < 3; i++)
            {
                temp[i] = rand.Next(3);
                for (int j = 0; j < i; j++)
                    if (temp[i] == temp[j]) i--;
            }
        }
        /// <summary>
        /// Object 그리기 메서드
        /// </summary>
        /// <param name="e"></param>
        public void draw(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            for (int i = 0; i < 4; i++)
            {
                if (scretRect[i] != deletRect)
                {
                    g.DrawImage(scret, scretRect[i], new Rectangle(0, 0, 35, 35), GraphicsUnit.Pixel);
                }

            }

            // --- 아이템 그리기 ---
            foreach (var item in activeItems)
            {
                if (item._IsActive)
                {
                    item.Draw(g);
                }
            }

            for (int i = 0; i < 3; i++)
            {
                // 일반 적이 죽지 않았을 때만 그림 (enemyHP[i] > 0으로 확인)
                if (enemyHP[i] > 0)
                {
                    g.DrawImage(bitmap[i], x, baseY[temp[i]]);
                    enemyRect[i] = new Rectangle(x + 5, baseY[temp[i]] + 10, 25, 20);
                }
                else
                {
                    enemyRect[i] = deletRect; // 죽은 적은 충돌 범위도 제거
                }
            }

            // 비밀 항아리 적이 있고, 깜박이지 않으며, 죽지 않았을 때만 그림 (enemyHP[3] > 0으로 확인)
            if (mode && !twinkle && enemyHP[3] > 0)
            {
                g.DrawImage(bitmap[3], x1, scretPo.Top);
            }
            else if (mode && enemyHP[3] <= 0)
            {
                scretColRect = deletRect;
            }

            if (scoreAni)
            {
                g.DrawString("1000", font, Brushes.White, scorePo.Left, scorePo.Top);
            }
        }
        /// <summary>
        /// 몬스터 및 적 간의 충돌 감지 및 움직임 메서드
        /// </summary>
        /// <param name="chRect"></param>
        public void move(Rectangle chRect)
        {
            if (x <= 51)
            {
                right = true;
                left = false;
            }
            if (x >= 690)
            {
                right = false;
                left = true;
            }
            if (right)
                x++;
            if (left)
                x--;

            if (mode)
            {
                // 비밀 항아리 적이 살아있을 때만 충돌 영역 업데이트 (enemyHP[3] > 0으로 확인)
                if (enemyHP[3] > 0)
                {
                    scretColRect = new Rectangle(x1 + 5, scretPo.Top + 10, 25, 25);
                }
                else
                {
                    scretColRect = deletRect; // 죽은 적은 충돌 범위 제거
                }

                if (!mode1)
                    x1++;
                if (x1 >= 690)
                    mode1 = true;

                if (x1 <= 51 && mode1)
                {
                    right1 = true;
                    left1 = false;
                }
                if (x1 >= 690 && mode1)
                {
                    right1 = false;
                    left1 = true;
                }
                if (right1)
                    x1++;
                if (left1)
                    x1--;
            }

            for (int i = 0; i < 3; i++)
            {
                // 일반 적이 살아있을 때만 캐릭터와의 충돌 검사 (enemyHP[i] > 0으로 확인)
                if (enemyHP[i] > 0 && chRect.IntersectsWith(enemyRect[i]))
                    gameOver = true;
            }
            // 비밀 항아리 적이 살아있을 때만 캐릭터와의 충돌 검사 (enemyHP[3] > 0으로 확인)
            if (mode && enemyHP[3] > 0 && chRect.IntersectsWith(scretColRect) && respone)
                gameOver = true;

            ani();
            screteCol(chRect);
        }
        /// <summary>
        /// 비밀 항아리 아이템 생성 랜덤 및 충돌 처리 메서드
        /// </summary>
        /// <param name="chRect"></param>
        void screteCol(Rectangle chRect) //여기에 아이템 추가하기.
        {
            for (int i = 0; i < 4; i++)
            {
                if (scretRect[i] != deletRect && chRect.IntersectsWith(scretRect[i]))
                {
                    scretRand = rand.Next(3);
                    if (scretRand == 0 && !mode)
                    {
                        mode = true;
                        scretPo = scretRect[i];
                        x1 = scretPo.Left;
                        enemyHP[3] = INITIAL_ENEMY_HP; // 비밀 항아리 적 생성 시 체력 초기화
                    }else if (scretRand == 1)
                    {
                        Items newItem = new Items(scretRect[i].X, scretRect[i].Y, Items.ItemType.SpeedUp); // 스피드업 아이템 생성
                        activeItems.Add(newItem);
                    }
                    else if(scretRand == 2)
                    {
                        scoreAni = true;
                        score += 1000;
                        scorePo = scretRect[i];
                    }
                    scretRect[i] = deletRect;
                }
            }
        }
        /// <summary>
        /// 점수 표시 시간 및 비밀 항아리 몬스터 깜빡임, 무적시간, 적 이동 애니메이션 메서드
        /// </summary>
        void ani()
        {
            time++;
            if (right)
            {
                if (time > 3)
                {
                    bitmap[0] = Properties.Resources.rightEnemy1_1_;
                    bitmap[1] = Properties.Resources.rightEnemy_1_;
                    bitmap[2] = Properties.Resources.rightEnemy_1_;
                }
                if (time > 6)
                {
                    bitmap[0] = Properties.Resources.rightEnemy;
                    bitmap[1] = Properties.Resources.rightEnemy1;
                    bitmap[2] = Properties.Resources.rightEnemy1;
                    time = 0;
                }
            }
            if (left)
            {
                if (time > 3)
                {
                    bitmap[0] = Properties.Resources.leftEnemy1_1_;
                    bitmap[1] = Properties.Resources.leftEnemy_1_;
                    bitmap[2] = Properties.Resources.leftEnemy_1_;
                }
                if (time > 6)
                {
                    bitmap[0] = Properties.Resources.leftEnemy;
                    bitmap[1] = Properties.Resources.leftEnemy1;
                    bitmap[2] = Properties.Resources.leftEnemy1;
                    time = 0;
                }
            }

            if (mode)
            {
                time1++;
                if (!mode1)
                {
                    if (time1 > 3)
                        bitmap[3] = Properties.Resources.rightEnemy1_1_;
                    if (time1 > 6)
                    {
                        bitmap[3] = Properties.Resources.rightEnemy;
                        time1 = 0;
                    }
                }
                if (left1)
                {
                    if (time1 > 3)
                        bitmap[3] = Properties.Resources.leftEnemy1_1_;
                    if (time1 > 6)
                    {
                        bitmap[3] = Properties.Resources.leftEnemy;
                        time1 = 0;
                    }
                }
                if (right1)
                {
                    if (time1 > 3)
                        bitmap[3] = Properties.Resources.rightEnemy1_1_;
                    if (time1 > 6)
                    {
                        bitmap[3] = Properties.Resources.rightEnemy;
                        time1 = 0;
                    }
                }
                time2++;
                time3++;
                if (time2 > 3 && time3 < 60)
                    twinkle = true;
                if (time2 > 6 && time3 < 60)
                {
                    twinkle = false;
                    time2 = 0;
                }
                if (time3 > 120)
                {
                    twinkle = false;
                    respone = true;
                }

            }

            if (scoreAni)
            {
                time4++;
                if (time4 > 50)
                {
                    scoreAni = false;
                    time4 = 0;
                }
            }
        }
        /// <summary>
        /// 적 공격 데미지 체크 메서드
        /// </summary>
        /// <param name="enemyIndex"></param>
        public void TakeDamage(int enemyIndex)
        {
            if (enemyIndex >= 0 && enemyIndex < enemyHP.Length) // 유효한 인덱스 범위 확인
            {
                if (enemyHP[enemyIndex] > 0)
                {
                    enemyHP[enemyIndex]--; // 체력 감소
                    if (enemyHP[enemyIndex] <= 0)
                    {
                        // 적이 죽었을 때 처리
                        if (enemyIndex >= 0 && enemyIndex < 3) // 일반 적
                        {
                            enemyRect[enemyIndex] = deletRect; // 해당 적의 충돌 범위 제거
                            score += 100; // 점수 획득
                        }
                        else if (enemyIndex == 3) // 비밀 항아리에서 나온 적
                        {
                            mode = false; // 몬스터 비활성화
                            mode1 = false; // 이동 방향
                            respone = false; //무적시간
                            time2 = 0; // 애니메이션/깜빡임 타이머
                            time3 = 0; // 무적 시간
                            score += 500; // 점수
                        }
                    }
                }
            }
        }
    }
}