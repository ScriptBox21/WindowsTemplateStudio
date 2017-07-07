﻿// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************

using System;
using System.IO;
using System.Security;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Templates.Core.Locations
{
    public sealed class LocalTemplatesSource : TemplatesSource
    {
        public string LocalTemplatesVersion { get; private set; }
        public string LocalWizardVersion { get; private set; }
        protected override bool VerifyPackageSignatures => false;
        public override bool ForcedAcquisition { get => base.ForcedAcquisition; protected set => base.ForcedAcquisition = value; }
        public string Origin => $@"..\..\..\..\..\{SourceFolderName}";

        private object lockObject = new object();

        public LocalTemplatesSource() : this("0.0.0.0", "0.0.0.0")
        {
            base.ForcedAcquisition = true;
        }

        public LocalTemplatesSource(string wizardVersion, string templatesVersion, bool forcedAdquisition = true)
        {
            base.ForcedAcquisition = forcedAdquisition;
            LocalTemplatesVersion = templatesVersion;
            LocalWizardVersion = wizardVersion;
        }

        protected override string AcquireMstx()
        {
            // Compress Content adding version return templatex path.
            var tempFolder = Path.Combine(GetTempFolder(), SourceFolderName);

            Copy(Origin, tempFolder);

            File.WriteAllText(Path.Combine(tempFolder, "version.txt"), LocalTemplatesVersion);

            return Templatex.Pack(tempFolder);
        }

        private void Copy(string sourceFolder, string targetFolder)
        {
            Fs.SafeDeleteDirectory(targetFolder);
            Fs.CopyRecursive(sourceFolder, targetFolder);
        }
    }
}
