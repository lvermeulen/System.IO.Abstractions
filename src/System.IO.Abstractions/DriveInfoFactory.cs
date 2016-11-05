namespace System.IO.Abstractions
{
    [Serializable]
    internal class DriveInfoFactory : IDriveInfoFactory
    {
        /// <summary>
        /// Retrieves the drive names of all logical drives on a computer.
        /// </summary>
        /// <returns>An array of type <see cref="DriveInfoBase"/> that represents the logical drives on a computer.</returns>
        public DriveInfoBase[] GetDrives()
        {
            DriveInfo[] driveInfos = DriveInfo.GetDrives();
            DriveInfoBase[] driveInfoWrappers = new DriveInfoBase[driveInfos.Length];
            for (int index = 0; index < driveInfos.Length; index++)
            {
                DriveInfo driveInfo = driveInfos[index];
                driveInfoWrappers[index] = new DriveInfoWrapper(driveInfo);
            }

            return driveInfoWrappers;
        }
    }
}
