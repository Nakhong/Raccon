using System;
using System.Drawing;
using System.Windows.Forms;

namespace Raccoon
{
    public class Acorn
    {
        public Rectangle rect;
        private Image acornImage;
        private int speed = 10;
        private int direction; // 0 :왼쪽/ 1 :오른쪽
        public bool IsActive { get; set; } // 도토리 활성화
        private int startX; // 도토리의 시작 X 위치
        private const int MAX_ATTACK_DISTANCE = 300; // 최대 공격 거리

        public Acorn(int startX, float startY, int dir)
        {
            acornImage = Properties.Resources.Acorn;
            rect = new Rectangle(startX, (int)startY, 20, 20);
            direction = dir;
            IsActive = true;
            this.startX = startX; // 시작 X 위치 저장
        }

        public void action(Enemy enemy)
        {
            // 1. 도토리 이동 로직
            if (direction == 0) // 왼쪽으로 이동
            {
                rect.X -= speed;
            }
            else // 오른쪽으로 이동
            {
                rect.X += speed;
            }

            // 2. 비활성화 조건
            // 화면을 벗어나면 비활성화
            if (rect.X < -rect.Width || rect.X > 800)
            {
                IsActive = false;
            }

            // 최대 공격 거리 제한 로직
            if (Math.Abs(rect.X - startX) > MAX_ATTACK_DISTANCE)
            {
                IsActive = false;
            }

            // 충돌 검사
            if (IsActive) // 아직 활성 상태일 때만 충돌 검사
            {
                // 일반 적과의 충돌
                for (int j = 0; j < 3; j++)
                {
                    if (enemy.enemyRect[j] != new Rectangle(0, 0, 0, 0) && this.rect.IntersectsWith(enemy.enemyRect[j]))
                    {
                        enemy.TakeDamage(j);
                        this.IsActive = false; // 도토리 비활성화
                        break; // 더 이상 충돌 검사할 필요 없음
                    }
                }

                if (enemy.mode && !enemy.twinkle && enemy.scretColRect != new Rectangle(0, 0, 0, 0) && this.rect.IntersectsWith(enemy.scretColRect))
                {
                    enemy.TakeDamage(3);
                    this.IsActive = false; // 도토리 비활성화
                }
            }
        }
        // 도토리 그리기
        public void Draw(Graphics g)
        {
            if (IsActive)
            {
                g.DrawImage(acornImage, rect);
            }
        }
    }
}