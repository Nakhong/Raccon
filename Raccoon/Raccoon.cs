using Raccoon.Model;
using Raccoon.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Raccoon
{
    /// <summary>
    /// 메인 폼
    /// </summary>
    public partial class Raccoon : Form
    {
        private Bitmap backGround; // 배경
        public Bitmap _BackGround
        {
            get {
                return backGround;
            }
            set
            {
                backGround = value;
            }
        }
        private Character character; // 배경
        public Character _Character
        {
            get
            {
                return character;
            }
            set
            {
                character = value;
            }
        }
        private Obstruction obstruction; // 배경
        public Obstruction _Obstruction
        {
            get
            {
                return obstruction;
            }
            set
            {
                obstruction = value;
            }
        }
        private bool stop; // 배경
        public bool _Stop
        {
            get
            {
                return stop;
            }
            set
            {
                stop = value;
            }
        }
        private Enemy enemy; // 배경
        public Enemy _Enemy
        {
            get
            {
                return enemy;
            }
            set
            {
                enemy = value;
            }
        }
        Ui _ui; // UI
        private Timer time; // 배경
        public Timer _Time
        {
            get
            {
                return time;
            }
            set
            {
                time = value;
            }
        }

        private List<Acorn> _activeAcorns; // 활성화 된 도토리 리스트
        private int _attackCooldown = 0; // 공격 쿨다운 타이머
        private const int MAX_ATTACK_COOLDOWN = 20; // 공격 쿨다운 (프레임 단위)

        private bool _isSpeedBoostActive; // 스피드 부스트 활성화 여부
        private int _speedBoostTimer; // 스피드 부스트 타이머
        private const int SPEED_BOOST_DURATION_FRAMES = 300; // 스피드 부스트 지속 시간
        private PauseMenuControl _pauseMenu;
        /// <summary>
        /// 게임 타이머 초기화 및 시작
        /// </summary>
        private void Time()
        {
            _Time = new Timer();
            _Time.Interval = 1;
            _Time.Tick += new EventHandler(Timer);
            _Time.Start();
        }
        /// <summary>
        /// 자연스럽게 흘러가도록 화면 로드 해주는 메서드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer(object sender, EventArgs e)
        {
            _Character.move();
            _Character.ani(_Obstruction._Gameover, _Enemy._GameOver);
            _Obstruction.collision(_Character._ChRect);
            _ui.addData(_Obstruction._Score, _Enemy._Score, _Obstruction._Count);
            _Enemy.move(_Character._ChRect);
            UpdateAcorns(); //도토리 업데이트
            Invalidate();
        }
        /// <summary>
        /// 폼 생성자
        /// </summary>
        public Raccoon()
        {
            _BackGround = Properties.Resources.BackGround; // 배경
            _Character = new Character(); // 캐릭터
            _Obstruction = new Obstruction(); // 방해물
            _Enemy = new Enemy(); // 적
            _ui = new Ui(); // 인터페이스
            _activeAcorns = new List<Acorn>(); //도토리
            _isSpeedBoostActive = false; // 초기 스피드 부스트 비활성화
            _speedBoostTimer = 0; // 초기 타이머 값
            SetStyle(System.Windows.Forms.ControlStyles.DoubleBuffer | System.Windows.Forms.ControlStyles.UserPaint | System.Windows.Forms.ControlStyles.AllPaintingInWmPaint, true);
            Time(); //게임 타이머 시작
            _pauseMenu = new PauseMenuControl(this); // Control에 보내기
            InitializeComponent(); // 컴포넌트 초기화
        }
        /// <summary>
        /// paint : 윈도우가 처음 화면에 표시되면 이벤트가 호출 된다.
        /// 폼 paint 이벤트 메서드 -> invalidate() 호출 시 paint 이벤트 실행
        /// 배경, 장애물, ui, 몬스터, 캐릭터 도토리 그리는 메서드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.DrawImage(_BackGround, 0, 0);
            _Obstruction.draw(e);
            _ui.draw(e);
            _Enemy.draw(e);
            _Character.draw(e);
            foreach (Acorn acorn in _activeAcorns)
            {
                acorn.Draw(g);
            }
        }
        /// <summary>
        /// 키보드 눌렸을 때 이벤트
        /// 캐릭터 이동 상태, 점프, 게임 일시정지, 재시작, 도토리 공격 처리
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
                _Character._State = 1;
            else if (e.KeyCode == Keys.Right)
                _Character._State = 2;
            else if (e.KeyCode == Keys.Up)
                _Character._State = 3;
            else if (e.KeyCode == Keys.Down)
                _Character._State = 4;
            else if (e.KeyCode == Keys.Space && _Character._Jump == 0 && !_Character._XStop && !_Character._JumpStop)
            {
                _Character._Jump++;
                _Character._JumpKey = true;
            }
            if (e.KeyCode == Keys.P)
            {
                stop = !stop;
            }
            if (stop)
            {
                _Time.Stop();
            }
            else
            {
                _Time.Start();
            }
            if (e.KeyCode == Keys.S)
            {
                _Character.reSet();
                _Obstruction.reSet();
                _Enemy.reSet();
                _ui.str1 = "";
            }
            if (e.KeyCode == Keys.Z && _attackCooldown == 0) //도토리 공격 추가
            {
                Point attackPos = _Character.GetAttackPosition();
                Acorn newAcorn = new Acorn(attackPos.X, attackPos.Y, _Character._Dir);
                _activeAcorns.Add(newAcorn); // 새 도토리를 리스트에 추가
                _attackCooldown = MAX_ATTACK_COOLDOWN; // 쿨다운 설정
            }
        }
        /// <summary>
        /// 키보드 떼어졌을 때 이벤트
        /// 캐릭터 이동 상태 초기화
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (_Character._State == 1 && e.KeyCode == Keys.Left)
            {
                _Character._State = 0;
            }
            if (_Character._State == 2 && e.KeyCode == Keys.Right)
            {
                _Character._State = 0;

            }
            if (_Character._State == 3 && e.KeyCode == Keys.Up)
            {
                _Character._State = 0;
            }
            if (_Character._State == 4 && e.KeyCode == Keys.Down)
            {
                _Character ._State = 0;
            }
        }
        /// <summary>
        /// 도토리 상태 업데이트 메서드
        /// </summary>
        private void UpdateAcorns()
        {
            for (int i = _activeAcorns.Count - 1; i >= 0; i--)
            {
                Acorn acorn = _activeAcorns[i];
                acorn.action(_Enemy);

                if (!acorn._IsActive)
                {
                    _activeAcorns.RemoveAt(i);
                }
            }

            if (_attackCooldown > 0) // 어택 쿨타임
            {
                _attackCooldown--;
            }
        }
    }
}