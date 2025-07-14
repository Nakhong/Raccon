using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raccoon.Model
{
    class Items
    {

        public Rectangle Rect { get; private set; } 
        public Bitmap Image { get; private set; }   
        public ItemType Type { get; private set; }  
        public bool IsActive { get; set; }          

        public enum ItemType
        {
            SpeedUp
        }

        public Items(int x, int y, ItemType type)
        {
            Rect = new Rectangle(x, y, 20, 20);
            Type = type;
            IsActive = true; 

            switch (type)
            {
                case ItemType.SpeedUp:
                    Image = Properties.Resources.speedUp; // 예시: speedUpItem 이미지를 Properties.Resources에 추가했다고 가정
                    break;
                default:
                    Image = Properties.Resources.speedUp; // 기본 이미지
                    break;
            }
        }
        
        public void Draw(Graphics g)
        {
            if (IsActive)
            {
                g.DrawImage(Image, Rect);
            }
        }
        
        public void Deactivate()
        {
            IsActive = false;
            Rect = new Rectangle(0, 0, 0, 0); // 충돌 영역 제거
        }
    }
}

