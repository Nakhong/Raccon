using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raccoon.Model
{
    /// <summary>
    /// 아이템 클래스
    /// </summary>
    public class Items
    {
        private Rectangle rect;
        public Rectangle _Rect
        {
            get
            {
                return rect;
            }
            set
            {
                rect = value;
            }
        }
        private Bitmap image;
        public Bitmap _Image
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
            }
        }
        private ItemType type;
        public ItemType _Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }
        private bool isActive;
        public bool _IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                isActive = value;
            }
        }


        public enum ItemType
        {
            SpeedUp
        }

        public Items(int x, int y, ItemType type)
        {
            _Rect = new Rectangle(x, y, 20, 20);
            _Type = type;
            _IsActive = true; 

            switch (type)
            {
                case ItemType.SpeedUp:
                    _Image = Properties.Resources.speedUp; 
                    break;
                default:
                    _Image = Properties.Resources.speedUp;
                    break;
            }
        }
        
        public void Draw(Graphics g)
        {
            if (_IsActive)
            {
                g.DrawImage(_Image, _Rect);
            }
        }
        
        public void Deactivate()
        {
            _IsActive = false;
            _Rect = new Rectangle(0, 0, 0, 0); // 충돌 영역 제거
        }
        //아이템 타이머 넣기
    }
}

