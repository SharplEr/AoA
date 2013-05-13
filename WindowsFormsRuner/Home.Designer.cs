namespace WindowsFormsRuner
{
    partial class Home
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.testList = new System.Windows.Forms.ListView();
            this.resultList = new System.Windows.Forms.ListView();
            this.StartLearn = new System.Windows.Forms.Button();
            this.AddFile = new System.Windows.Forms.Button();
            this.Check = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // testList
            // 
            this.testList.Location = new System.Drawing.Point(12, 36);
            this.testList.Name = "testList";
            this.testList.Size = new System.Drawing.Size(239, 277);
            this.testList.TabIndex = 0;
            this.testList.UseCompatibleStateImageBehavior = false;
            this.testList.View = System.Windows.Forms.View.List;
            // 
            // resultList
            // 
            this.resultList.Location = new System.Drawing.Point(308, 36);
            this.resultList.Name = "resultList";
            this.resultList.Size = new System.Drawing.Size(239, 277);
            this.resultList.TabIndex = 0;
            this.resultList.UseCompatibleStateImageBehavior = false;
            this.resultList.View = System.Windows.Forms.View.List;
            // 
            // StartLearn
            // 
            this.StartLearn.Location = new System.Drawing.Point(167, 319);
            this.StartLearn.Name = "StartLearn";
            this.StartLearn.Size = new System.Drawing.Size(84, 34);
            this.StartLearn.TabIndex = 1;
            this.StartLearn.Text = "Обучение";
            this.StartLearn.UseVisualStyleBackColor = true;
            this.StartLearn.Click += new System.EventHandler(this.StartLearn_Click);
            // 
            // AddFile
            // 
            this.AddFile.Location = new System.Drawing.Point(12, 320);
            this.AddFile.Name = "AddFile";
            this.AddFile.Size = new System.Drawing.Size(84, 34);
            this.AddFile.TabIndex = 1;
            this.AddFile.Text = "Добавить";
            this.AddFile.UseVisualStyleBackColor = true;
            this.AddFile.Click += new System.EventHandler(this.AddFile_Click);
            // 
            // Check
            // 
            this.Check.Enabled = false;
            this.Check.Location = new System.Drawing.Point(308, 320);
            this.Check.Name = "Check";
            this.Check.Size = new System.Drawing.Size(156, 34);
            this.Check.TabIndex = 1;
            this.Check.Text = "Проверить из файла";
            this.Check.UseVisualStyleBackColor = true;
            this.Check.Click += new System.EventHandler(this.Check_Click);
            // 
            // Home
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(572, 366);
            this.Controls.Add(this.AddFile);
            this.Controls.Add(this.Check);
            this.Controls.Add(this.StartLearn);
            this.Controls.Add(this.resultList);
            this.Controls.Add(this.testList);
            this.Name = "Home";
            this.Text = "Home";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView testList;
        private System.Windows.Forms.ListView resultList;
        private System.Windows.Forms.Button StartLearn;
        private System.Windows.Forms.Button AddFile;
        private System.Windows.Forms.Button Check;
    }
}

