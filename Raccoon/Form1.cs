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
    public partial class Form1 : Form
    {
        Bitmap backGround;
        Character character;
        Obstruction obstruction;
        Ui ui;
        Enemy enemy;
        Timer time;
        bool stop;
        private List<Acorn> activeAcorns;
        private int attackCooldown = 0; // 공격 쿨다운 타이머
        private const int MAX_ATTACK_COOLDOWN = 20; // 공격 쿨다운 (프레임 단위)

        private void Time()
        {
            time = new Timer();
            time.Interval = 1;
            time.Tick += new EventHandler(timer);
            time.Start();
        }

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

        public Form1()
        {
            backGround = Properties.Resources.BackGround; // 배경
            character = new Character(); // 캐릭터
            obstruction = new Obstruction(); // 방해물
            enemy = new Enemy(); // 적
            ui = new Ui(); // 인터페이스
            activeAcorns = new List<Acorn>(); //도토리
            SetStyle(System.Windows.Forms.ControlStyles.DoubleBuffer | System.Windows.Forms.ControlStyles.UserPaint | System.Windows.Forms.ControlStyles.AllPaintingInWmPaint, true);
            Time();
            InitializeComponent();
        }

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
        //도토리 공격 업데이트
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