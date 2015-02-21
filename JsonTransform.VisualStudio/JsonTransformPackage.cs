using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Utilities;

namespace JsonTransform.VisualStudio
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [Guid(GuidList.guidJsonTransform_VisualStudioPkgString)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    internal sealed class JsonTransformPackage : Package
    {
        [Export]
        [FileExtension(".json-patch")]
        [ContentType("json")]
        internal FileExtensionToContentTypeDefinition JsonPatchFileExtensionToContentTypeDefinition;

        private static DTE2 _dte;

        public static DTE2 DTE
        {
            get { return _dte; }
        }

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public JsonTransformPackage()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }



        /////////////////////////////////////////////////////////////////////////////
        // Overridden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Debug.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));

            var bin = new Uri(typeof(JsonTransformPackage).Assembly.CodeBase, UriKind.Absolute).LocalPath;
            var binDir = Path.GetDirectoryName(bin);
            var targetDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "JSON Document Transforms");
            Directory.CreateDirectory(targetDir);
            
            var sourceLocation = Path.Combine(binDir, "JsonTransform.exe");
            var targetLocation = Path.Combine(targetDir, "JsonTransform.exe");
            File.Copy(sourceLocation, targetLocation, true);

            sourceLocation = Path.Combine(binDir, "JsonTransform.Core.dll");
            targetLocation = Path.Combine(targetDir, "JsonTransform.Core.dll");
            File.Copy(sourceLocation, targetLocation, true);

            sourceLocation = Path.Combine(binDir, "Newtonsoft.Json.dll");
            targetLocation = Path.Combine(targetDir, "Newtonsoft.Json.dll");
            File.Copy(sourceLocation, targetLocation, true);

            sourceLocation = Path.Combine(binDir, "JsonTransform.targets");
            targetLocation = Path.Combine(targetDir, "JsonTransform.targets");
            File.Copy(sourceLocation, targetLocation, true);

            base.Initialize();
        }

        #endregion

    }
}
