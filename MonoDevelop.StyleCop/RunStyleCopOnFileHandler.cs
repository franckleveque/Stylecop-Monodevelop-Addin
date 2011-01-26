namespace MonoDevelop.StyleCop
{
    using System;
    using System.Collections.Generic;

    using Microsoft.StyleCop;

    using MonoDevelop.Components.Commands;
    using MonoDevelop.Ide;
    using MonoDevelop.Ide.Gui;
    using MonoDevelop.Ide.Gui.Pads;
    using MonoDevelop.Ide.Tasks;

    /// <summary>
    /// Class used to handle a single file from Stylecop
    /// </summary>
    public class RunStyleCopOnFileHandler : CommandHandler
    {
        #region Fields

        /// <summary>
        /// Static object defining ownership of Task objects
        /// </summary>
        private static object styleCopAddinOwner = new object();

        /// <summary>
        /// Pad containing errors
        /// </summary>
        private ErrorListPad errorPad = null;

        /// <summary>
        /// Output logger of Stylecop messages
        /// </summary>
        private System.IO.TextWriter logger = null;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the RunStyleCopOnFileHandler class.
        /// </summary>
        public RunStyleCopOnFileHandler()
        {
            Pad tmpPad = IdeApp.Workbench.Pads.Find(
                              delegate(Pad toCheck)
                              {
                                return toCheck.Id.Equals("MonoDevelop.Ide.Gui.Pads.ErrorListPad");
                              });
            if (tmpPad != null)
            {
                this.errorPad = tmpPad.Content as ErrorListPad;
            }

            this.logger = this.errorPad.GetBuildProgressMonitor().Log;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Run the stylecop analyse on the file
        /// </summary>
        protected override void Run()
        {
            base.Run();
            IdeApp.Workbench.ActiveDocument.Save();

            TaskService.Errors.ClearByOwner(RunStyleCopOnFileHandler.styleCopAddinOwner);
            this.errorPad.ShowResults(this, new EventArgs());

            StyleCopConsole console = new StyleCopConsole(null, true, null, null, true);
            Configuration configuration = new Configuration(new string[] { "DEBUG" });

            List<CodeProject> projects = new List<CodeProject>();
            CodeProject project = new CodeProject(IdeApp.ProjectOperations.CurrentSelectedProject.BaseDirectory.GetHashCode(), IdeApp.ProjectOperations.CurrentSelectedProject.BaseDirectory, configuration);

            // Add each source file to this project.
            console.Core.Environment.AddSourceCode(project, IdeApp.Workbench.ActiveDocument.FileName, null);
            projects.Add(project);

            console.OutputGenerated += this.OnOutputGenerated;
            console.ViolationEncountered += this.OnViolationEncountered;
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            console.Start(projects, false);
            console.OutputGenerated -= this.OnOutputGenerated;
            console.ViolationEncountered -= this.OnViolationEncountered;
            console.Dispose();
        }

        /// <summary>
        /// Update availability of command
        /// </summary>
        /// <param name="info">
        /// A <see cref="CommandInfo"/>
        /// </param>
        protected override void Update(CommandInfo info)
        {
            base.Update(info);
            info.Enabled = System.IO.Path.GetExtension(IdeApp.Workbench.ActiveDocument.FileName) == ".cs";
        }

        /// <summary>
        /// Generate an output message for Stylecop informations messages
        /// </summary>
        /// <param name="sender">
        /// A <see cref="System.Object"/> determining which object sends this output message
        /// </param>
        /// <param name="args">
        /// A <see cref="OutputEventArgs"/> determining elements composing the message
        /// </param>
        private void OnOutputGenerated(object sender, OutputEventArgs args)
        {
            // For the moment, Nothing to do
            if (this.logger != null)
            {
                this.logger.WriteLine(args.Output);
            }
        }

        /// <summary>
        /// Reports violations encountered by stylecop to the error pad
        /// </summary>
        /// <param name="sender">
        /// A <see cref="System.Object"/> determining which object sends this violation
        /// </param>
        /// <param name="args">
        /// A <see cref="ViolationEventArgs"/> determining elements composing the violation
        /// </param>
        private void OnViolationEncountered(object sender, ViolationEventArgs args)
        {
            try
            {
            MonoDevelop.Projects.BuildError b = new MonoDevelop.Projects.BuildError(args.Element.Document.SourceCode.Path, args.LineNumber, 0, args.Violation.Rule.CheckId, args.Message);
            b.IsWarning = args.Warning;
            Task errorTask = new Task(b, RunStyleCopOnFileHandler.styleCopAddinOwner);

            this.errorPad.AddTask(errorTask);
            }
            catch (Exception e)
            {
                this.logger.WriteLine("Error: {0}\nStackTrace:{1}", e.Message, e.StackTrace);
            }
        }

        #endregion Methods
    }
}