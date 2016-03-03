namespace Glass.LeadTools.Recognition
{
    using System.ComponentModel;
    using Leadtools;

    public class LeadToolsLicenseApplier : ILeadToolsLicenseApplier
    {
        public void ApplyLicense()
        {
            string licenseFilePath = System.IO.Path.GetFullPath(@"LeadToolsLicense\Bravent-REC19.lic");
            string keyFilePath = System.IO.Path.GetFullPath(@"LeadToolsLicense\Bravent-REC19.lic.key");
            if (System.IO.File.Exists(licenseFilePath) && System.IO.File.Exists(keyFilePath))
            {
                string developerKey = System.IO.File.ReadAllText(keyFilePath);
                RasterSupport.SetLicense(licenseFilePath, developerKey);
            }
            else
            {
                throw new LicenseException(
                    typeof(RasterSupport),
                    null,
                    string.Format("No se han encontrado los archivos de licencia necesarios:\n{0}\n{1}", licenseFilePath, keyFilePath));
            }

            if (RasterSupport.KernelExpired)
            {
                var exMessage = "La licencia es inválida o ha experado. El componente de LeadTools se ha deshabilitado. Por favor, contacte con el soporte técnico de LeadTools para conseguir una licencia válida.";
                throw new LicenseException(typeof(RasterSupport), null, exMessage);
            }
        }
    }
}