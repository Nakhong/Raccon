using System;
using System.Drawing;
using System.Windows.Forms;

namespace Raccoon
{
    /// <summary>
    /// 도토리 관련 클래스
    /// </summary>
    public class Acorn
    {
        private Rectangle rect; // 도토리 위치 크기
        public Rectangle _rect
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
        private bool IsActive; // 활성화 체크
        public bool _IsActive
        {
            get
            {
                return IsActive;
            }
            set
            {
                IsActive = value;
            }
        }
        private Image _acornImage; // 이미지
        private int _speed = 10; // 날아가는 속도
        private int _direction; // 방향 (0 :왼쪽/ 1 :오른쪽)
        private int _startX; // 도토리의 시작 X 위치
        private const int MAX_ATTACK_DISTANCE = 300; // 최대 공격 거리
        /// <summary>
        /// 도토리 생성자
        /// </summary>
        /// <param name="startX">도토리 초기 X 값</param>
        /// <param name="startY">도토리 초기 Y 값</param>
        /// <param name="dir">도토리 방향</param>
        public Acorn(int startX, float startY, int dir)
        {
            _acornImage = Properties.Resources.Acorn;
            rect = new Rectangle(startX, (int)startY, 20, 20);
            _direction = dir;
            IsActive = true;
            this._startX = startX; // 시작 X 위치 저장
        }
        /// <summary>
        /// 도토리의 이동, 적 충돌, 데미지, 사거리 메서드
        /// </summary>
        /// <param name="enemy"></param>
        public void action(Enemy enemy)
        {
            // 1. 도토리 이동 로직
            if (_direction == 0) // 왼쪽으로 이동
            {
                rect.X -= _speed;
            }
            else // 오른쪽으로 이동
            {
                rect.X += _speed;
            }

            // 2. 비활성화 조건
            // 화면을 벗어나면 비활성화
            if (rect.X < -rect.Width || rect.X > 800)
            {
                IsActive = false;
            }

            // 최대 공격 거리 제한 로직
            if (Math.Abs(rect.X - _startX) > MAX_ATTACK_DISTANCE)
            {
                IsActive = false;
            }

            // 충돌 검사
            if (IsActive) // 아직 활성 상태일 때만 충돌 검사
            {
                // 일반 적과의 충돌
                for (int j = 0; j < 3; j++)
                {
                    if (enemy._EnemyRect[j] != new Rectangle(0, 0, 0, 0) && this.rect.IntersectsWith(enemy._EnemyRect[j]))
                    {
                        enemy.TakeDamage(j);
                        this.IsActive = false; // 도토리 비활성화
                        break; // 더 이상 충돌 검사할 필요 없음
                    }
                }

                if (enemy._Mode && !enemy._Twinkle && enemy._ScretColRect != new Rectangle(0, 0, 0, 0) && this.rect.IntersectsWith(enemy._ScretColRect))
                {
                    enemy.TakeDamage(3);
                    this.IsActive = false; // 도토리 비활성화
                }
            }
        }
        /// <summary>
        /// 도토리 활성화 상태일 경우, 게임 화면에 도토리 그리기
        /// </summary>
        /// <param name="g"></param>
        public void Draw(Graphics g)
        {
            if (IsActive)
            {
                g.DrawImage(_acornImage, rect);
            }
        }
    }
}