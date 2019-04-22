using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        List<Action> ActionsList = new List<Action>();
        double simulationTime = 0;
        bool draw = false;
        public Form1()
        {
            InitializeComponent();
            pictureBox1.Paint += new PaintEventHandler(this.pictureBox1_Paint);
            button3.Enabled = false;
            button4.Enabled = false;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.Visible = false;
            this.Text = "Production design - network analysis using the CMP + Gantt chart";
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, 0);
            this.Size = new Size(1450, 820);
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            Form2 ActionForm = new Form2(listView1,button3);
            ActionForm.Show();        
        }

        private void button2_Click(object sender, EventArgs e)
        {
            while(listView1.SelectedItems.Count>0)
            {
                listView1.SelectedItems[0].Remove();
            }
            if (listView1.Items.Count<1)
            {
                button3.Enabled = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            bool noError = true;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = true;
            int numberOfEvents = 0;
            for (int i=0;i<listView1.Items.Count;i++)
            {
                Action temp = new Action();
                temp.name = listView1.Items[i].SubItems[0].Text;
                temp.previous = int.Parse(listView1.Items[i].SubItems[1].Text);
                temp.next = int.Parse(listView1.Items[i].SubItems[2].Text);
                temp.time = double.Parse(listView1.Items[i].SubItems[3].Text);
                temp.isCritical = false;
                temp.startTime = 0;
                temp.nextActions = new List<int>();
                if (temp.next>numberOfEvents)
                {
                    numberOfEvents = temp.next;
                }
                ActionsList.Add(temp);
            }
            List<Event> events = new List<Event>();
            for (int i = 0; i < numberOfEvents; i++)
            {
                Event t = new Event();
                events.Add(t);
            }
            for (int i = 0; i < ActionsList.Count; i++)
            {
                events[ActionsList[i].previous-1].nextEvent.Add(ActionsList[i].next-1);
                events[ActionsList[i].previous-1].nextAction.Add(i);
                events[ActionsList[i].next-1].previousEvent.Add(ActionsList[i].previous-1);
                events[ActionsList[i].next-1].previousAction.Add(i);
                for (int j = 0; j < ActionsList.Count; j++)
                {
                    if(ActionsList[i].next == ActionsList[j].previous)
                    {
                        ActionsList[i].nextActions.Add(j);
                    }
                    
                }
            }
            for (int i = 1; i < numberOfEvents; i++)
            {
                if (events[i-1].nextEvent.Count == 0 || events[i].previousEvent.Count == 0)
                {
                    MessageBox.Show("DATA ERROR");
                    noError = false;
                    break;
                }
            }
            if(noError)
            {
                for (int i = 1; i < numberOfEvents; i++)
                {
                    double tempTime = 0;
                    for (int j = 0; j < events[i].previousEvent.Count; j++)
                    {
                        tempTime = events[events[i].previousEvent[j]].earliestTime + ActionsList[events[i].previousAction[j]].time;
                        if (tempTime > events[i].earliestTime)
                        {
                            events[i].earliestTime = tempTime;
                        }
                    }
                }
                events[numberOfEvents - 1].latestTime = events[numberOfEvents - 1].earliestTime;
                for (int i = numberOfEvents - 2; i > 0; i--)
                {
                    events[i].latestTime = events[numberOfEvents - 1].earliestTime;
                    double tempTime = events[numberOfEvents - 1].earliestTime;
                    for (int j = 0; j < events[i].nextEvent.Count; j++)
                    {
                        tempTime = events[events[i].nextEvent[j]].latestTime - ActionsList[events[i].nextAction[j]].time;
                        if (tempTime < events[i].latestTime)
                        {
                            events[i].latestTime = tempTime;
                        }
                    }
                }
                for (int i = 0; i < numberOfEvents; i++)
                {
                    events[i].timeGap = events[i].latestTime - events[i].earliestTime;
                }
                List<List<int>> criticalPath = new List<List<int>>();
                List<int> criticalPathStartingEvent = new List<int>();
                criticalPathStartingEvent.Add(0);
                criticalPath.Add(new List<int>());
                for (int i = 0; i < criticalPath.Count; i++)
                {
                    int j = criticalPathStartingEvent[i];
                    int t = 0;
                    int z = 0;
                    while (j != (numberOfEvents - 1))
                    {
                        z = 0;
                        for (int k = 0; k < events[j].nextEvent.Count; k++)
                        {
                            if (events[events[j].nextEvent[k]].timeGap == 0)
                            {
                                if (z == 0)
                                {
                                    criticalPath[i].Add(events[j].nextAction[k]);
                                    z = 1;
                                    t = events[j].nextEvent[k];
                                }
                                else
                                {
                                    criticalPath.Add(new List<int>());
                                    for (int x = 0; x < (criticalPath[i].Count - 1); x++)
                                    {
                                        criticalPath[criticalPath.Count - 1].Add(criticalPath[i][x]);
                                    }
                                    criticalPathStartingEvent.Add(events[j].nextEvent[k]);
                                    criticalPath[criticalPath.Count - 1].Add(events[j].nextAction[k]);
                                }
                            }
                        }
                        j = t;
                        if (z == 0)
                        {
                            criticalPath.RemoveAt(i);
                            criticalPathStartingEvent.RemoveAt(i);
                            i--;
                            j = (numberOfEvents - 1);
                        }
                    }

                }
                for (int i = 0; i < criticalPath.Count; i++)
                {
                    double time = 0;
                    for (int x = 0; x < criticalPath[i].Count; x++)
                    {
                        time += ActionsList[criticalPath[i][x]].time;
                    }
                    if (time != events[numberOfEvents - 1].latestTime)
                    {
                        criticalPath.RemoveAt(i);
                        i--;
                    }
                }
                string ex;
                simulationTime = events[numberOfEvents - 1].latestTime;
                if (criticalPath.Count > 1)
                {
                    label2.Text = "Critical paths:";
                }
                else
                {
                    label2.Text = "Critical path:";
                }
                for (int j = 0; j < criticalPath.Count; j++)
                {
                    ex = "1";
                    for (int i = 0; i < criticalPath[j].Count; i++)
                    {
                        ActionsList[criticalPath[j][i]].isCritical = true;
                        ex += "-";
                        ex += ActionsList[criticalPath[j][i]].next;
                    }
                    comboBox1.Items.Add(ex);
                }
                for (int i = 0; i < ActionsList.Count; i++)
                {
                    ActionsList[i].startTime = events[ActionsList[i].previous - 1].earliestTime;
                }
                ex = "Duration: ";
                ex += simulationTime.ToString();
                label1.Text = ex;
                comboBox1.Visible = true;
                draw = true;
                pictureBox1.Invalidate();
            }
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = false;
            ActionsList.Clear();
            simulationTime = 0;
            label1.Text = "";
            label2.Text = "";
            comboBox1.Items.Clear();
            comboBox1.Visible = false;
            pictureBox1.Image = null;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (draw)
            {
                SolidBrush blueBrush = new SolidBrush(Color.FromArgb(170, 0, 0, 255));
                SolidBrush redBrush = new SolidBrush(Color.FromArgb(160, 255, 0, 0));
                SolidBrush blackBrush = new SolidBrush(Color.FromArgb(255, 0, 0, 0));
                Pen redPen = new Pen(Color.Red, 1);
                Pen bluePen = new Pen(Color.Blue, 1);
                Pen blackPen = new Pen(Color.Black, 3);
                Pen greyPen = new Pen(Color.FromArgb(30, 0, 0, 0), 1);

                Font nameFont = new Font("Arial", 10);
                Font timeFont = new Font("Arial", 6);
                Graphics g = e.Graphics;
                int heigh = (pictureBox1.Height - 12)/ ActionsList.Count;
                int width = (int)((pictureBox1.Width - 100) / (simulationTime+0.2));
                for(int i=0;i< ActionsList.Count; i++)
                {
                    g.DrawLine(greyPen, 0, heigh * i, pictureBox1.Width, heigh * i);
                    g.DrawString(ActionsList[i].name, nameFont, blackBrush, new Point(1, (int)(heigh * (i+0.5)-6)));
                }
                g.DrawLine(greyPen, 0, heigh * ActionsList.Count, pictureBox1.Width, heigh * ActionsList.Count);
                for (int i = 0; i <= ((pictureBox1.Width - 100)/width); i++)
                {
                    g.DrawLine(greyPen, 100 + width*i, 0, 100 + width * i, pictureBox1.Height);
                    g.DrawString((i+1).ToString(), timeFont, blackBrush, new Point(101 + width * i, pictureBox1.Height - 10));
                }
                g.DrawLine(blackPen, 98, heigh* ActionsList.Count+2, pictureBox1.Width, heigh * ActionsList.Count+2);
                g.DrawLine(blackPen, 98, 0, 98, heigh * ActionsList.Count + 2);
                for (int i = 0; i < ActionsList.Count; i++)
                {
                    double x = width * ActionsList[i].startTime + 100;
                    double xx = width * ActionsList[i].time;
                    int y = heigh * i;
                    Rectangle ee = new Rectangle((int)(x), y, (int)(xx), (heigh));
                    if (ActionsList[i].isCritical)
                    {
                        g.FillRectangle(redBrush, ee);
                        g.DrawRectangle(redPen, ee);
                    }
                    else
                    {
                        g.FillRectangle(blueBrush, ee);
                        g.DrawRectangle(bluePen, ee);
                    }
                }
                List<int> criticalActionsList = new List<int>();
                for (int i = 0; i < ActionsList.Count; i++)
                {
                    if (ActionsList[i].isCritical)
                    {
                        criticalActionsList.Add(i);
                    }
                    else
                    {
                        SolidBrush brush = new SolidBrush(Color.Blue);
                        Pen pen = new Pen(Color.Blue, 3);
                        double x = width * ActionsList[i].startTime + 100;
                        double xx = width * ActionsList[i].time;
                        int y = heigh * i;
                        for (int j = 0; j < ActionsList[i].nextActions.Count; j++)
                        {
                            if(i<ActionsList[i].nextActions[j])
                            {
                                double time = ActionsList[ActionsList[i].nextActions[j]].startTime;
                                time = time - (ActionsList[i].startTime + ActionsList[i].time);
                                time = time * width;
                                int yy = heigh * ActionsList[i].nextActions[j];
                                g.DrawLine(pen, (int)(xx + x), (int)(y + heigh / 2), (int)(xx + x + 10 + time), (int)(y + heigh / 2));
                                g.DrawLine(pen, (int)(xx + x + 10 + time), (int)(y + heigh / 2), (int)(xx + x + 10 + time), yy - 8);
                                Point point1 = new Point((int)(xx + x + 10 + time), yy);
                                Point point2 = new Point((int)(xx + x + 10 + time + 5), (yy - 8));
                                Point point3 = new Point((int)(xx + x + 10 + time - 5), (yy - 8));
                                Point[] curvePoints = { point1, point2, point3 };
                                g.FillPolygon(brush, curvePoints);
                            }
                            else
                            {
                                double time = ActionsList[ActionsList[i].nextActions[j]].startTime;
                                time = time - (ActionsList[i].startTime + ActionsList[i].time);
                                time = time * width;
                                int yy = heigh * (ActionsList[i].nextActions[j]+1);
                                g.DrawLine(pen, (int)(xx + x), (int)(y + heigh / 2), (int)(xx + x + 10 + time), (int)(y + heigh / 2));
                                g.DrawLine(pen, (int)(xx + x + 10 + time), (int)(y + heigh / 2), (int)(xx + x + 10 + time), yy + 8);
                                Point point1 = new Point((int)(xx + x + 10 + time), yy);
                                Point point2 = new Point((int)(xx + x + 10 + time + 5), (yy + 8));
                                Point point3 = new Point((int)(xx + x + 10 + time - 5), (yy + 8));
                                Point[] curvePoints = { point1, point2, point3 };
                                g.FillPolygon(brush, curvePoints);
                            }
                        }
                    }
                }
                for (int i = 0; i < criticalActionsList.Count; i++)
                {
                    SolidBrush brush = new SolidBrush(Color.Red);
                    Pen pen = new Pen(Color.Red, 3);
                    double x = width * ActionsList[criticalActionsList[i]].startTime + 100;
                    double xx = width * ActionsList[criticalActionsList[i]].time;
                    int y = heigh * criticalActionsList[i];
                    for (int j = 0; j < ActionsList[criticalActionsList[i]].nextActions.Count; j++)
                    {
                        if(criticalActionsList[i] < ActionsList[criticalActionsList[i]].nextActions[j])
                        {
                            double time = ActionsList[ActionsList[criticalActionsList[i]].nextActions[j]].startTime;
                            time = time - (ActionsList[criticalActionsList[i]].startTime + ActionsList[criticalActionsList[i]].time);
                            time = time * width;
                            int yy = heigh * ActionsList[criticalActionsList[i]].nextActions[j];
                            g.DrawLine(pen, (int)(xx + x), (int)(y + heigh / 2), (int)(xx + x + 10 + time), (int)(y + heigh / 2));
                            g.DrawLine(pen, (int)(xx + x + 10 + time), (int)(y + heigh / 2), (int)(xx + x + 10 + time), yy - 8);
                            Point point1 = new Point((int)(xx + x + 10 + time), yy);
                            Point point2 = new Point((int)(xx + x + 10 + time + 5), (yy - 8));
                            Point point3 = new Point((int)(xx + x + 10 + time - 5), (yy - 8));
                            Point[] curvePoints = { point1, point2, point3 };
                            g.FillPolygon(brush, curvePoints);
                        }
                        else
                        {
                            double time = ActionsList[ActionsList[criticalActionsList[i]].nextActions[j]].startTime;
                            time = time - (ActionsList[criticalActionsList[i]].startTime + ActionsList[criticalActionsList[i]].time);
                            time = time * width;
                            int yy = heigh * (ActionsList[criticalActionsList[i]].nextActions[j]+1);
                            g.DrawLine(pen, (int)(xx + x), (int)(y + heigh / 2), (int)(xx + x + 10 + time), (int)(y + heigh / 2));
                            g.DrawLine(pen, (int)(xx + x + 10 + time), (int)(y + heigh / 2), (int)(xx + x + 10 + time), yy + 8);
                            Point point1 = new Point((int)(xx + x + 10 + time), yy);
                            Point point2 = new Point((int)(xx + x + 10 + time + 5), (yy + 8));
                            Point point3 = new Point((int)(xx + x + 10 + time - 5), (yy + 8));
                            Point[] curvePoints = { point1, point2, point3 };
                            g.FillPolygon(brush, curvePoints);
                        }
                       
                    }
                }
            }
            draw = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
