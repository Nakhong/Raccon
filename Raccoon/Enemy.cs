using System;
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
        Rectangle[] enemyRect = new Rectangle[3];
        Rectangle[] scretRect = new Rectangle[4];
        Rectangle deletRect = new Rectangle(0, 0, 0, 0);
        Rectangle scretPo, scorePo, scretColRect;        
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

        public Enemy()
        {
            yRandom();
            createScret();
            createFont();
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
        }

        void createFont()
        {
            privateFonts = new PrivateFontCollection();
            privateFonts.AddFontFile("neoletters.ttf");
            font = new Font(privateFonts.Families[0], 10f, FontStyle.Regular);
        }

        void createScret() //비밀항아리 아이템을 그려줄 영역
        {
            scret = Properties.Resources.scretItem;
            scretRect[0] = new Rectangle(450, 445, 35, 35);
            scretRect[1] = new Rectangle(370, 349, 35, 35);
            scretRect[2] = new Rectangle(85, 253, 35, 35);
            scretRect[3] = new Rectangle(680, 157, 35, 35);
        }

        void yRandom() //적으로 사용될 이미지 불러오기, 적이 위치할 y좌표를 중복없이 랜덤으로 생성
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
                    if (temp[i] == temp[j]) i--;  //앞과 뒤의 숫자가 같으면 i를 한번 감소시킴으로서 상위 for문을 한번더 실행시킴
            }


        }

        public void draw(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            for (int i = 0; i < 4; i++)
                g.DrawImage(scret, scretRect[i], new Rectangle(0, 0, 35, 35), GraphicsUnit.Pixel);  //비빌항아리 아이템을 그려줌
            for (int i = 0; i < 3; i++)
            {
                g.DrawImage(bitmap[i], x, baseY[temp[i]]);     //temp의 좌표에 적을 그림, 
                enemyRect[i] = new Rectangle(x + 5, baseY[temp[i]]+10, 25, 20);  //적의 충돌범위 
                //g.DrawRectangle(new Pen(Brushes.Red), enemyRect[i]);
            }
            if (mode && !twinkle)     //twinkle을 true flase로 게속 바꿔줌으로서 비밀항아리에서 나온 적과는 일정시간동안 충돌하지않는다는걸보여줌
                g.DrawImage(bitmap[3], x1, scretPo.Top);
            if (scoreAni)    //socreAni가 true일 동안만 비밀항아리 아이템의 점수를 보여줌
                g.DrawString("1000", font, Brushes.White, scorePo.Left, scorePo.Top);
            //g.DrawRectangle(new Pen(Brushes.Red), scretColRect);
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
            if (right)  //right가 true면 적은 우측으로이동
                x++;
            if (left)  //left가 true면 적은 좌측으로 이동
                x--;

            if (mode)  //항아리에서 적이나오게되면 true가됨
            {
                scretColRect = new Rectangle(x1 + 5, scretPo.Top+10, 25, 25);
                if (!mode1)  //처음에 적이나오면 우측으로 감
                    x1++;
                if (x1 >= 690)    //항아리에서 나온적이 우측끝에 도달하면 mode1은 false
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
                if (right1)   //right1이 true면 적은 우측으로이동
                    x1++;
                if (left1)   //left1이 true면 적은 좌측으로 이동
                    x1--;                
            }
           
            for (int i = 0; i < 3; i++)
            {
                if (chRect.IntersectsWith(enemyRect[i]))  //캐릭터랑 적이랑 부딪힐때
                    gameOver = true;
            }
            if (mode && chRect.IntersectsWith(scretColRect) && respone)   //캐릭터랑 비밀항아리에서 나온 적이랑 부딪힐때
                gameOver = true;

            ani();           
            screteCol(chRect);      //비밀항아리와 캐릭터의 충돌함수
        }        

        void screteCol(Rectangle chRect)
        {
            for (int i = 0; i < 4; i++)
            {
                if (chRect.IntersectsWith(scretRect[i])) //캐릭터와 비밀항아리가 충돌했을때
                {
                    scretRand = rand.Next(2);  //0과1둘중에 하나가 생성되는 난수를 만들어줌
                    if (scretRand == 0 && !mode)  //0이면, 즉 2분의1확률로 적이나옴 
                    {
                        mode = true;            //mode를 true로 바꿈으로써 항아리와 충돌시 적이 나오지 않게설정
                        scretPo = scretRect[i];  //충돌한 항아리의 좌표를 scretPo에 저장
                        x1 = scretPo.Left;     //충돌한 항아리의 좌측끝의 좌표를 x1에 저장
                    }
                    else       //비밀항아리에서 적이 나오지 않을시에는 1000점 획득
                    { 
                        scoreAni = true;
                        score += 1000;
                        scorePo = scretRect[i];
                    }
                    scretRect[i] = deletRect;  //충돌한 비밀항아리의 영역전체를 0으로설정함으로써 없앰
                }
            }
        }
     
        void ani()
        {
            time++;
            if (right)     //적이 우측으로 이동시 보여주는애니
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
            if (left)   //적이 좌측으로 이동시 보여주는애니
            {
                if (time > 3)
                {
                    bitmap[0] = Properties.Resources.leftEnemy1_1_;     //2층에 있는 적의 첫번째 애니
                    bitmap[1] = Properties.Resources.leftEnemy_1_;    //3층에 있는 적의 첫번째 애니
                    bitmap[2] = Properties.Resources.leftEnemy_1_;   //5층에있는 적의 첫번째 애니
                }
                if (time > 6)
                {
                    bitmap[0] = Properties.Resources.leftEnemy;   //2층에 있는 적의 두번째 애니
                    bitmap[1] = Properties.Resources.leftEnemy1;  //3층에 있는 적의 두번째 애니
                    bitmap[2] = Properties.Resources.leftEnemy1;  //5층에있는 적의 두번째 애니
                    time = 0;
                }
            }

            if(mode)   //비밀항아리와 충돌했을때 나온 적의 애니
            {
                time1++;
                if (!mode1)  //처음 비밀항아리에서 나온적이 우측끝까지 가는동안 보여줄 애니
                {
                    if (time1 > 3)
                        bitmap[3] = Properties.Resources.rightEnemy1_1_;
                    if (time1 > 6)
                    { 
                        bitmap[3] = Properties.Resources.rightEnemy;
                        time1 = 0;
                    }
                }     
                if(left1)  //비밀항아리에서 나온적이 좌측으로가는 애니
                {
                    if(time1 > 3)
                        bitmap[3] = Properties.Resources.leftEnemy1_1_;
                    if (time1 > 6)
                    {
                        bitmap[3] = Properties.Resources.leftEnemy;
                        time1 = 0;
                    }
                }
                if(right1)  //비밀항아리에서 나온적이 우측으로가는 애니
                {
                    if(time1 > 3)
                        bitmap[3] = Properties.Resources.rightEnemy1_1_;
                    if(time1 > 6)
                    {
                        bitmap[3] = Properties.Resources.rightEnemy;
                        time1 = 0;
                    }
                }
                time2++;
                time3++;
                if (time2 > 3 && time3 < 60)  
                    twinkle = true;
                if (time2 > 6 && time3 < 60)  //비밀항아리에서 나온적을 일정시간동안 깜박거리게 해줌
                {
                    twinkle = false;
                    time2 = 0;
                }
                if (time3 > 120)  //time3가 120이되면은 깜박거리는거와 무적시간을 없앰
                {
                    twinkle = false;
                    respone = true;
                }
                
            }

            if(scoreAni)
            {
                time4++;
                if(time4 > 50)   //time4가 50이될동안만 획득한 아이템의 점수를 보여줌
                {
                    scoreAni = false;
                    time4 = 0;
                }
            }

        }
    }
}