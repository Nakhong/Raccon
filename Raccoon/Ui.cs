using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Text;

namespace Raccoon
{
    public class Ui
    {
        Font font, font1;
        PrivateFontCollection privateFonts;
        public String str, str1;                

        public Ui()
        {
            createFont();            
        } 

        void createFont() //폰트생성
        {
            privateFonts = new PrivateFontCollection();
            privateFonts.AddFontFile("neoletters.ttf");
            font = new Font(privateFonts.Families[0], 24f, FontStyle.Regular);
            font1 = new Font(privateFonts.Families[0], 17f, FontStyle.Regular);
        }

        public void draw(PaintEventArgs e)
        {            
            Graphics g = e.Graphics;           
            g.DrawString(str, font, Brushes.White, 100, 30);
            g.DrawString("다시시작 버튼 : S", font1, Brushes.White, 330, 15);
            g.DrawString("일시정지 : P", font1, Brushes.White, 330, 45);
            g.DrawString(str1, font1, Brushes.White, 280, 320);
        }       

        public void addData(int score, int score1, int count)  // Obstruction, Enemy클래스에서 score의 변수를 실시간으로 받아와 해당socre에 더해줌
        {
            score += score1;
            str = "SCORE : " + score.ToString();
            if (count == 10)  // count는 획득한 아이템의수 
                str1 = "클리어 다시시작하려면 : S";
        }

    }
}