using Calendar;
using LiteDB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp;

namespace MySchedulerCalendar
{



    public partial class CalendarForm : Form
    {
        List<Appointment> m_Appointments;


        public CalendarForm()
        {
            m_Appointments = new List<Appointment>();
            using (var db = new LiteDatabase(new ConnectionString("test.litedb")))
            {
                var collection = db.GetCollection<Appointment>("appointment");
                var enddate = DateTime.Now.AddDays(7);
                var results = collection.FindAll();
                foreach (var r in results)
                {
                    r.BorderColor = Color.FromArgb(r.ARGBorderColor);
                    r.TextColor = Color.FromArgb(r.ARGBTextColor);
                    r.Color = Color.FromArgb(r.ARGBColor);
                    m_Appointments.Add(r);
                }

            }
            InitializeComponent();
            dayView1.StartDate = DateTime.Now;
            dateTimePicker1.Value = DateTime.Now.AddDays(-1);
            dateTimePicker2.Value = DateTime.Now.AddMonths(1);
            dayView1.DaysToShow = 7;
            dayView1.NewAppointment += new NewAppointmentEventHandler(DayView1_NewAppointment);
            dayView1.SelectionChanged += new EventHandler(DayView1_SelectionChanged);
            dayView1.ResolveAppointments += new Calendar.ResolveAppointmentsEventHandler(this.DayView1_ResolveAppointments);
            dayView1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DayView1_MouseMove);
            dayView1.MouseClick += new MouseEventHandler(DayView1_MouseClick);


        }

        private void Form1_Closing(object sender, FormClosingEventArgs e)
        {
            try
            {
                using (var db = new LiteDatabase(new ConnectionString("test.litedb")))
                {
                    var collection = db.GetCollection<Appointment>("appointment");
                    foreach (var m_App in m_Appointments)
                        collection.Update(m_App);
                }
            }
            catch (Exception ex)
            {
                Log.AppendOnFile("E", "Error on update items:{0}", ex.Message);

                if (ex.InnerException != null)
                {
                    Log.AppendOnFile("E", "Inner Exception:{0}", ex.InnerException.Message);
                }
            }
        }

        private void DayView1_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (dayView1.SelectedAppointment == null)
                {

                    if (e.Button == MouseButtons.Left)
                    {
                        Appointment m_App = new Appointment
                        {
                            StartDate = dayView1.SelectionStart,
                            EndDate = dayView1.SelectionEnd,
                            BorderColor = Color.Red,
                            Color = Color.White,
                            ARGBColor = Color.White.ToArgb(),
                            ARGBorderColor=Color.Red.ToArgb(),
                             
                    };
                     
                        m_Appointments.Add(m_App);
                        using (var db = new LiteDatabase(new ConnectionString("test.litedb")))
                        {
                            var collection = db.GetCollection<Appointment>("appointment");

                            collection.Insert(m_App);
                        }
                        dayView1.Invalidate();
                    }
                }
                else
                {
                    if (e.Button == MouseButtons.Right)
                    {

                        ContextMenuStrip menuStrip = new ContextMenuStrip();

                        ToolStripMenuItem menuItem = new ToolStripMenuItem("Color");



                        menuItem.Click += new EventHandler(ColorItem_Click);

                        menuItem.Name = "Color";

                        menuStrip.Items.Add(menuItem);
                        menuItem = new ToolStripMenuItem("Border");



                        menuItem.Click += new EventHandler(BorderItem_Click);

                        menuItem.Name = "Border";

                        menuStrip.Items.Add(menuItem);
                        menuItem = new ToolStripMenuItem("Delete");



                        menuItem.Click += new EventHandler(DeleteItem_Click);

                        menuItem.Name = "Delete";

                        menuStrip.Items.Add(menuItem);
                        dayView1.ContextMenuStrip = menuStrip;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.AppendOnFile("E", "Error on mouse click event:{1}", ex.Message);
                if (ex.InnerException != null)
                {
                    Log.AppendOnFile("E", "Inner Exception:{0}", ex.InnerException.Message);
                }


            }
        }

        private void DeleteItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (dayView1.SelectedAppointment != null)
                {
                    var m_A = m_Appointments.Find(m => m.ID == dayView1.SelectedAppointment.ID);
                    m_Appointments.Remove(m_A);
                    using (var db = new LiteDatabase(new ConnectionString("test.litedb")))
                    {
                        var collection = db.GetCollection<Appointment>("appointment");
                        collection.Delete(m_A.ID);
                    }
                }
                dayView1.ContextMenuStrip = new ContextMenuStrip();
                dayView1.Invalidate();
            }
            catch (Exception ex)
            {
                Log.AppendOnFile("E", "Error removing Selected Appointment:{0},{1}", dayView1.SelectedAppointment.Title, ex.Message);
                if (ex.InnerException != null)
                {
                    Log.AppendOnFile("E", "Inner Exception:{0}", ex.InnerException.Message);
                }
            }
        }

        private void BorderItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (dayView1.SelectedAppointment != null)
                {
                    colorDialog1.Color = dayView1.SelectedAppointment.BorderColor;

                    if (colorDialog1.ShowDialog(this) == DialogResult.OK)
                    {
                        dayView1.SelectedAppointment.BorderColor = colorDialog1.Color;
                        var m_A = m_Appointments.Find(m => m.ID == dayView1.SelectedAppointment.ID);
                        m_A.BorderColor = colorDialog1.Color;
                        m_A.ARGBorderColor = colorDialog1.Color.ToArgb();
                        using (var db = new LiteDatabase(new ConnectionString("test.litedb")))
                        {
                            var collection = db.GetCollection<Appointment>("appointment");
                            collection.Update(m_A);
                        }
                    }
                }
                dayView1.ContextMenuStrip = new ContextMenuStrip();
                dayView1.Invalidate();
            }
            catch (Exception ex)
            {
                Log.AppendOnFile("E", "Error on bordercolor change:{0},{1}", dayView1.SelectedAppointment.Title, ex.Message);
                if (ex.InnerException != null)
                {
                    Log.AppendOnFile("E", "Inner Exception:{0}", ex.InnerException.Message);
                }
            }
        }

        private void ColorItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (dayView1.SelectedAppointment != null)
                {
                    colorDialog1.Color = dayView1.SelectedAppointment.Color;

                    if (colorDialog1.ShowDialog(this) == DialogResult.OK)
                    {
                        dayView1.SelectedAppointment.Color = colorDialog1.Color;
                        var m_A = m_Appointments.Find(m => m.ID == dayView1.SelectedAppointment.ID);
                        m_A.Color = colorDialog1.Color;
                        m_A.ARGBColor = colorDialog1.Color.ToArgb();
                        using (var db = new LiteDatabase(new ConnectionString("test.litedb")))
                        {
                            var collection = db.GetCollection<Appointment>("appointment");
                            collection.Update(m_A);
                        }
                    }
                }

                dayView1.ContextMenuStrip = new ContextMenuStrip();
                dayView1.Invalidate();
            }
            catch (Exception ex)
            {
                Log.AppendOnFile("E", "Error on color change:{0},{1}", dayView1.SelectedAppointment.Title, ex.Message);
                if (ex.InnerException != null)
                {
                    Log.AppendOnFile("E", "Inner Exception:{0}", ex.InnerException.Message);
                }
            }
        }

        private void DayView1_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                lblTime.Text = dayView1.GetTimeAt(e.X, e.Y).ToString();
            }
            catch (Exception ex)
            {
                Log.AppendOnFile("E", "Error mouse move event:{0}", ex.Message);
                if (ex.InnerException != null)
                {
                    Log.AppendOnFile("E", "Inner Exception:{0}", ex.InnerException.Message);
                }
            }
        }

        private void DayView1_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                lblDay.Text = dayView1.SelectionStart.ToString() + ":" + dayView1.SelectionEnd.ToString();
            }
            catch (Exception ex)
            {
                Log.AppendOnFile("E", "Error changing selection:{0}", ex.Message);
                if (ex.InnerException != null)
                {
                    Log.AppendOnFile("E", "Inner Exception:{0}", ex.InnerException.Message);
                }
            }
        }

        private void DayView1_ResolveAppointments(object sender, ResolveAppointmentsEventArgs args)
        {
            try
            {
                List<Appointment> m_Apps = new List<Appointment>();

                foreach (Appointment m_App in m_Appointments)
                    if ((m_App.StartDate >= args.StartDate) &&
                        (m_App.StartDate <= args.EndDate))
                        m_Apps.Add(m_App);

                args.Appointments = m_Apps;
            }
            catch (Exception ex)
            {
                Log.AppendOnFile("E", "Error resolving appointment:{0}", ex.Message);
                if (ex.InnerException != null)
                {
                    Log.AppendOnFile("E", "Inner Exception:{0}", ex.InnerException.Message);
                }
            }
        }



        private void DayView1_NewAppointment(object sender, NewAppointmentEventArgs args)
        {
            try
            {
                Appointment m_App = new Appointment
                {
                    StartDate = dayView1.SelectionStart,
                    EndDate = dayView1.SelectionEnd,
                    BorderColor = Color.Red,
                    Color = Color.White
                };
                m_Appointments.Add(m_App);
            }
            catch (Exception ex)
            {
                Log.AppendOnFile("E", "Error on new appointment:{0}", ex.Message);
                if (ex.InnerException != null)
                {
                    Log.AppendOnFile("E", "Inner Exception:{0}", ex.InnerException.Message);
                }
            }
        }


        private void MonthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            try
            {
                // m_Appointments.Clear();
                dayView1.StartDate = monthCalendar1.SelectionStart;
                /*  using (LiteDatabase db = new LiteDatabase(new ConnectionString("test.litedb")))
                  {
                      var collection = db.GetCollection<Appointment>("appointment");
                      var enddate = dayView1.StartDate.AddDays(7);
                      var results = collection.Find(x => x.StartDate >= dayView1.StartDate && x.EndDate <= enddate);

                      foreach (var r in results)
                          m_Appointments.Add(r);

                  }*/

            }
            catch (Exception ex)
            {
                Log.AppendOnFile("E", "Error changing months:{0}", ex.Message);
                if (ex.InnerException != null)
                {
                    Log.AppendOnFile("E", "Inner Exception:{0}", ex.InnerException.Message);
                }
            }
        }

        private void BtnReport_Click(object sender, EventArgs e)
        {
            try
            {
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = DateTime.Now.ToString("dd-MM-yyyy") + "-Export";
                saveFileDialog.DefaultExt = ".pdf";
                saveFileDialog.Filter = "Pdf Report (.pdf)|*.pdf";
                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                var document = new PdfDocument();

                var page = document.AddPage();
                var gfx = XGraphics.FromPdfPage(page);
                var font = new XFont("Times New Roman", 10, XFontStyle.Bold);

                var tf = new XTextFormatter(gfx);

                using (var db = new LiteDatabase(new ConnectionString("test.litedb")))
                {
                    var collection = db.GetCollection<Appointment>("appointment");
                    var enddate = dateTimePicker2.Value;
                    var results = collection.Find(r => r.StartDate >= dateTimePicker1.Value && r.EndDate <= enddate).OrderBy(m => m.StartDate);

                    int x, y;
                    x = 40;
                    y = 100;

                    foreach (var r in results)
                    {

                        var text = String.Format("{0}-{1}[{2}]", r.Title, r.StartDate, r.EndDate.ToString("HH:mm"));
                        var rect = new XRect(x, y, 500, 30);


                        if (y > page.Height)
                        {
                            page = document.AddPage();
                        }
                        XColor xColor = XColor.FromArgb(r.ARGBColor);
                        gfx.DrawRectangle(new XSolidBrush(xColor), rect);


                        tf.Alignment = XParagraphAlignment.Justify;
                        tf.DrawString(text, font, XBrushes.Black, rect);
                        XColor aColor = XColor.FromArgb(r.ARGBorderColor);
                        XPen xPen = new XPen(aColor);
                        gfx.DrawRectangle(xPen, rect);
                        y += 31;
                    }
                }
                // Save the document...
                document.Save(saveFileDialog.FileName);
                // ...and start a viewer.
                var p = new System.Diagnostics.Process();
                p.StartInfo.FileName = saveFileDialog.FileName;
                p.Start();

                p.WaitForExit();



            }
            catch (Exception ex)
            {
                Log.AppendOnFile("E", "Error generating reports:{0}", ex.Message);
                if (ex.InnerException != null)
                {
                    Log.AppendOnFile("E", "Inner Exception:{0}", ex.InnerException.Message);
                }
            }
        }
    }
}
