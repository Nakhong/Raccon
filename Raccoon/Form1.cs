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
    public partial class Form1 : Form
    {
        Bitmap backGround; // 배경
        Character character; // 캐릭터
        Obstruction obstruction; // 장애물
        Ui ui; // UI
        Enemy enemy; // 적
        Timer time; // 업데이트 타이머
        bool stop; // 게임 정지
        private List<Acorn> activeAcorns; // 활성화 된 도토리 리스트
        private int attackCooldown = 0; // 공격 쿨다운 타이머
        private const int MAX_ATTACK_COOLDOWN = 20; // 공격 쿨다운 (프레임 단위)
        /// <summary>
        /// 게임 타이머 초기화 및 시작 일종의 THREAD 느낌?
        /// </summary>
        private void Time()
        {
            time = new Timer();
            time.Interval = 1;
            time.Tick += new EventHandler(timer);
            time.Start();
        }
        /// <summary>
        /// 자연스럽게 흘러가도록 화면 그려주는 메서드 일종의 JOB 느낌 ?
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer(object sender, EventArgs e)
        {
            character.move();
            character.ani(obstruction._Gameover, enemy._GameOver);
            obstruction.collision(character._ChRect);
            ui.addData(obstruction._Score, enemy._Score, obstruction._Count);
            enemy.move(character._ChRect);
            UpdateAcorns(); //도토리 업데이트
            Invalidate();
        }
        /// <summary>
        /// 폼 생성자
        /// </summary>
        public Form1()
        {
            backGround = Properties.Resources.BackGround; // 배경
            character = new Character(); // 캐릭터
            obstruction = new Obstruction(); // 방해물
            enemy = new Enemy(); // 적
            ui = new Ui(); // 인터페이스
            activeAcorns = new List<Acorn>(); //도토리
            SetStyle(System.Windows.Forms.ControlStyles.DoubleBuffer | System.Windows.Forms.ControlStyles.UserPaint | System.Windows.Forms.ControlStyles.AllPaintingInWmPaint, true);
            Time(); //게임 타이머 시작
            InitializeComponent(); // 컴포넌트 초기화
        }
        /// <summary>
        /// 폼 paint 이벤트 메서트
        /// 배경, 장애물, ui, 몬스터, 캐릭터 도토리 그리는 메서드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.DrawImage(backGround, 0, 0);
            obstruction.draw(e);
            ui.draw(e);
            enemy.draw(e);
            character.draw(e);
            //도토리 그리기
            foreach (Acorn acorn in activeAcorns)
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
                character._State = 1;
            else if (e.KeyCode == Keys.Right)
                character._State = 2;
            else if (e.KeyCode == Keys.Up)
                character._State = 3;
            else if (e.KeyCode == Keys.Down)
                character._State = 4;
            else if (e.KeyCode == Keys.Space && character._Jump == 0 && !character._XStop && !character._JumpStop)
            {
                character._Jump++;
                character._JumpKey = true;
            }
            if (e.KeyCode == Keys.P)
                stop = !stop;
            if (stop)
                time.Stop();
            else
                time.Start();
            if (e.KeyCode == Keys.S)
            {
                character.reSet();
                obstruction.reSet();
                enemy.reSet();
                ui.str1 = "";
            }
            if (e.KeyCode == Keys.Z && attackCooldown == 0) //도토리 공격 추가
            {
                Point attackPos = character.GetAttackPosition();
                Acorn newAcorn = new Acorn(attackPos.X, attackPos.Y, character._Dir);
                activeAcorns.Add(newAcorn); // 새 도토리를 리스트에 추가
                attackCooldown = MAX_ATTACK_COOLDOWN; // 쿨다운 설정
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
            if (character._State == 1 && e.KeyCode == Keys.Left)
                character._State = 0;
            if (character._State == 2 && e.KeyCode == Keys.Right)
                character._State = 0;
            if (character._State == 3 && e.KeyCode == Keys.Up)
                character._State = 0;
            if (character._State == 4 && e.KeyCode == Keys.Down)
                character._State = 0;
        }
        /// <summary>
        /// 도토리 상태 업데이트 메서드
        /// </summary>
        private void UpdateAcorns()
        {
            for (int i = activeAcorns.Count - 1; i >= 0; i--)
            {
                Acorn acorn = activeAcorns[i];
                acorn.action(enemy);

                if (!acorn._IsActive)
                {
                    activeAcorns.RemoveAt(i);
                }
            }

            if (attackCooldown > 0)
            {
                attackCooldown--;
            }
        }
    }
}