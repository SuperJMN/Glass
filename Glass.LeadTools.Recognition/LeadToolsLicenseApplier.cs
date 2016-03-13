namespace Glass.LeadTools.Recognition
{
    using System.ComponentModel;
    using Leadtools;

    public class LeadToolsLicenseApplier : ILeadToolsLicenseApplier
    {
        public void ApplyLicense()
        {
            string licenseFilePath = System.IO.Path.GetFullPath(@"LeadToolsLicense\License.lic");
            string keyFilePath = System.IO.Path.GetFullPath(@"LeadToolsLicense\License.lic.key");
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
                    $"No se han encontrado los archivos de licencia necesarios:\n{licenseFilePath}\n{keyFilePath}");
            }

            if (RasterSupport.KernelExpired)
            {
                var exMessage = "La licencia es inválida o ha expirado. El componente de LeadTools se ha deshabilitado. Por favor, contacte con el soporte técnico de LeadTools para conseguir una licencia válida.";
                throw new LicenseException(typeof(RasterSupport), null, exMessage);
            }
        }
    }
}