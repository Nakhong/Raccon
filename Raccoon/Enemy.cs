using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Raccoon
{
    public class Enemy
    {
        Random rand = new Random();
        int[] temp = new int[3];
        int[] baseY = new int[3];
        int x = 51;
        int x1;
        Bitmap[] bitmap = new Bitmap[4];
        Bitmap scret;
        public Rectangle[] enemyRect = new Rectangle[3]; // 일반 적 3마리
        Rectangle[] scretRect = new Rectangle[4];
        Rectangle deletRect = new Rectangle(0, 0, 0, 0);
        public Rectangle scretPo, scorePo, scretColRect;
        public bool right, left, right1, left1, gameOver, mode, mode1, twinkle, respone, scoreAni;
        int time = 0;
        int time1 = 0;
        int time2 = 0;
        int time3 = 0;
        int time4 = 0;
        int scretRand;
        public int score = 0;
        Font font;
        PrivateFontCollection privateFonts;
        private int[] enemyHP = new int[4];
        private const int INITIAL_ENEMY_HP = 1;


        public Enemy()
        {
            yRandom();
            createScret();
            createFont();
            InitializeEnemyHP();
        }

        void InitializeEnemyHP()
        {
            for (int i = 0; i < enemyHP.Length; i++) // enemyHP 배열의 모든 요소 초기화
            {
                enemyHP[i] = INITIAL_ENEMY_HP;
            }

            for (int i = 0; i < 3; i++)
            {
                enemyRect[i] = new Rectangle(0, 0, 0, 0);
            }
            scretColRect = new Rectangle(0, 0, 0, 0);
        }

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
            InitializeEnemyHP();
        }

        void createFont()
        {
            privateFonts = new PrivateFontCollection();
            privateFonts.AddFontFile("neoletters.ttf");
            font = new Font(privateFonts.Families[0], 10f, FontStyle.Regular);
        }

        void createScret()
        {
            scret = Properties.Resources.scretItem;
            scretRect[0] = new Rectangle(450, 445, 35, 35);
            scretRect[1] = new Rectangle(370, 349, 35, 35);
            scretRect[2] = new Rectangle(85, 253, 35, 35);
            scretRect[3] = new Rectangle(680, 157, 35, 35);
        }

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

        void screteCol(Rectangle chRect)
        {
            for (int i = 0; i < 4; i++)
            {
                if (scretRect[i] != deletRect && chRect.IntersectsWith(scretRect[i]))
                {
                    scretRand = rand.Next(2);
                    if (scretRand == 0 && !mode)
                    {
                        mode = true;
                        scretPo = scretRect[i];
                        x1 = scretPo.Left;
                        enemyHP[3] = INITIAL_ENEMY_HP; // 비밀 항아리 적 생성 시 체력 초기화
                    }
                    else
                    {
                        scoreAni = true;
                        score += 1000;
                        scorePo = scretRect[i];
                    }
                    scretRect[i] = deletRect;
                }
            }
        }

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
                            mode = false;
                            mode1 = false;
                            respone = false;
                            time2 = 0;
                            time3 = 0;
                            score += 500;
                        }
                    }
                }
            }
        }
    }
}