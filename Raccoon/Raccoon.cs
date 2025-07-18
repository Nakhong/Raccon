using Raccoon.Model;
using Raccoon.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
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
        private bool _isGamePaused = false; //게임 일시정지

        private List<Acorn> _activeAcorns; // 활성화 된 도토리 리스트
        private int _attackCooldown = 0; // 공격 쿨다운 타이머
        private const int MAX_ATTACK_COOLDOWN = 20; // 공격 쿨다운 (프레임 단위)

        private bool _isSpeedBoostActive; // 스피드 부스트 활성화 여부
        private int _speedBoostTimer; // 스피드 부스트 타이머
        private const int SPEED_BOOST_DURATION_FRAMES = 300; // 스피드 부스트 지속 시간

        private PauseMenuControl _pauseMenu;
        private WMPLib.WindowsMediaPlayer bgm = new WMPLib.WindowsMediaPlayer(); // BGM 플레이어

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

            InitializeComponent(); // 컴포넌트 초기화
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
            BGMSetting();
            PauseMenuSetting();

        }
        /// <summary>
        /// esc 메뉴 세팅 메서드
        /// </summary>
        private void PauseMenuSetting()
        {
            _pauseMenu = new PauseMenuControl(this);

            // 중앙 위치 계산
            int centerX = (this.ClientSize.Width - _pauseMenu.Width) / 2;
            int centerY = (this.ClientSize.Height - _pauseMenu.Height) / 2;

            _pauseMenu.Location = new Point(centerX, centerY); // 계산된 위치 적용
            _pauseMenu.Visible = false; // 처음에 보이지 않게 설정

            this.Controls.Add(_pauseMenu); // 폼의 컨트롤 컬렉션에 추가
            _pauseMenu.BringToFront(); // 다른 컨트롤 위에 표시되도록 가장 앞으로 가져옴
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
            // 일시정지 상태에서는 Esc, P 키만 작동하도록 허용
            // S 키는 PauseMenuControl에서 직접 처리되므로, 여기서는 제외해도 됩니다.
            if (_isGamePaused && e.KeyCode != Keys.Escape && e.KeyCode != Keys.P)
            {
                return; // 일시정지 중에는 다른 키 입력 무시
            }

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
            //if (e.KeyCode == Keys.P)
            //{
            //    stop = !stop;
            //}
            //if (stop)
            //{
            //    _Time.Stop();
            //}
            //else
            //{
            //    _Time.Start();
            //}
            //if (e.KeyCode == Keys.S)
            //{
            //    _Character.reSet();
            //    _Obstruction.reSet();
            //    _Enemy.reSet();
            //    _ui.str1 = "";
            //}
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

            if (e.KeyCode == Keys.Escape)
            {
                if (_isGamePaused)
                {
                    ResumeGame(); // 게임 재개
                }
                else
                {
                    PauseGame(); // 게임 일시정지
                }
            }
        }
        /// <summary>
        /// 게임 다시 시작
        /// </summary>
        public void ResumeGame()
        {
            _isGamePaused = false;
            _Time.Start(); // 게임 타이머 다시 시작
            stop = false; // 게임 로직 재개
            _pauseMenu.Visible = false; // 메뉴 숨김
            this.Focus();
        }
        /// <summary>
        /// 게임 멈춤
        /// </summary>
        public void PauseGame()
        {
            _isGamePaused = true;
            _Time.Stop(); // 게임 타이머 정지
            stop = true; // 게임 로직 정지 (Timer 메서드 내에서 활용)
            _pauseMenu.Visible = true; // 메뉴 표시
            this.Focus();
        }

        /// <summary>
        /// 도토리 상태 업데이트 메서드
        /// </summary>
        private void UpdateAcorns()
        {
            for (int i = _activeAcorns.Count - 1; i >= 0; i--)
            {
                Acorn acorn = _activeAcorns[i]; // 도토리
                acorn.action(_Enemy); // 도토리 액션

                if (!acorn._IsActive) //도토리가 active 아니라면 삭제
                {
                    _activeAcorns.RemoveAt(i);
                }
            }

            if (_attackCooldown > 0) // 어택 쿨타임
            {
                _attackCooldown--;
            }
        }
        /// <summary>
        /// bgm 넣기
        /// </summary>
        private void BGMSetting()
        {

            bgm.URL = $"{AppDomain.CurrentDomain.BaseDirectory}//BGM//bgm.mp3";
            bgm.settings.setMode("loop", true);
            bgm.settings.volume = 100;
            bgm.controls.play();
        }
    }
}