using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FA_TOOL_SOFTWARE
{
    class shellZip
    {
	    #region Var
	    private FolderItems _shellItems;
	    private IEnumerable<ZipEntry> _items;
	    #endregion
	
	
	    #region Private Property
	    /// <summary>
	    /// Gets the m_ shell items.
	    /// </summary>
	    /// <value>The m_ shell items.</value>
	    private FolderItems m_ShellItems
	    {
	        get
	        {
	            return _shellItems??(_shellItems = (new Shell()).NameSpace(FilePath).Items());
	        }
	    }
	    #endregion
	
	
	    #region Property
	    /// <summary>
	    /// Gets or sets the file.
	    /// </summary>
	    /// <value>The file.</value>
	    public string FilePath { get; private set; }
	
	
	    /// <summary>
	    /// Gets the count.
	    /// </summary>
	    /// <value>The count.</value>
	    public int Count
	    {
	        get
	        {
	            return m_ShellItems.Count;
	        }
	    }
	
	
	    public IEnumerable<ZipEntry> Items
	    {
	        get
	        {
	            if (_items == null)
	            {
	                var items = new List<ZipEntry>();
	                foreach (FolderItem shellItem in m_ShellItems)
	                {
	                    items.Add(new ZipEntry(shellItem));
	                }
	                _items = items;
	            }
	            return _items;
	        }
	    }
	    #endregion
	
	
	
	    #region Constructor
	    /// <summary>
	    /// Initializes a new instance of the <see cref="ShellZip"/> class.
	    /// </summary>
	    /// <param name="zipFile">The zip file.</param>
        public shellZip(string zipFile)
	    {
	        this.FilePath = zipFile;
	    }
	    #endregion
	
	
	
	    #region Private Static Method
	    /// <summary>
	    /// Shells the copy to.
	    /// </summary>
	    /// <param name="from">From.</param>
	    /// <param name="to">To.</param>
	    private static void ShellCopyTo(string from, string to)
	    {
	        Shell sc = new Shell();
	        Folder SrcFolder = sc.NameSpace(from);
	        Folder DestFolder = sc.NameSpace(to);
	        FolderItems items = SrcFolder.Items();
	        DestFolder.CopyHere(items, 20);
	    }
	    #endregion
	
	
	
	    #region Public Static Method
	    /// <summary>
	    /// Compresses the specified source folder path.
	    /// </summary>
	    /// <param name="sourceFolderPath">The source folder path.</param>
	    /// <param name="zipFile">The zip file.</param>
	    public static void Compress(string sourceFolderPath, string zipFile)
	    {
	        if (!Directory.Exists(sourceFolderPath))
	            throw new DirectoryNotFoundException();
	
	        if (!File.Exists(zipFile))
	            File.Create(zipFile).Dispose();
	
	        ShellCopyTo(sourceFolderPath, zipFile);
	    }
	
	
	    /// <summary>
	    /// Des the compress.
	    /// </summary>
	    /// <param name="zipFile">The zip file.</param>
	    /// <param name="destinationFolderPath">The destination folder path.</param>
	    public static void DeCompress(string zipFile, string destinationFolderPath)
	    {
	        if (!File.Exists(zipFile))
	            throw new FileNotFoundException();
	
	        if (!Directory.Exists(destinationFolderPath))
	            Directory.CreateDirectory(destinationFolderPath);
	
	        ShellCopyTo(zipFile, destinationFolderPath);
	    }
	    #endregion
	
	
	    #region Public Method
	    /// <summary>
	    /// Des the compress.
	    /// </summary>
	    /// <param name="destinationFolderPath">The destination folder path.</param>
	    public void DeCompress(string destinationFolderPath)
	    {
	        DeCompress(this.FilePath, destinationFolderPath);
	    }
	    #endregion
    }//class shellzip


    public enum ZipEntryType
    {
        File,
        Folder
    }
    public class ZipEntry
    {
        #region Private Property
        private FolderItem m_ShellItem { get; set; }
        private IEnumerable<ZipEntry> _entrys;
        #endregion

        #region Public Property
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return m_ShellItem.Name;
            }
        }


        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public ZipEntryType Type
        {
            get
            {
                return m_ShellItem.IsFolder ? ZipEntryType.Folder : ZipEntryType.File;
            }
        }


        /// <summary>
        /// Gets the modify date.
        /// </summary>
        /// <value>The modify date.</value>
        public DateTime ModifyDate
        {
            get
            {
                return m_ShellItem.ModifyDate;
            }
        }


        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <value>The size.</value>
        public int Size
        {
            get
            {
                return m_ShellItem.Size;
            }
        }


        public IEnumerable<ZipEntry> Entrys
        {
            get
            {
                if (_entrys == null)
                {
                    if (!m_ShellItem.IsFolder)
                    {
                        _entrys = new ZipEntry[0];
                    }
                    else
                    {
                        var folder = m_ShellItem.GetFolder as Folder;
                        var items = new List<ZipEntry>();
                        foreach (FolderItem shellItem in folder.Items())
                        {
                            items.Add(new ZipEntry(shellItem));
                        }
                        _entrys = items;
                    }
                }
                return _entrys;
            }
        }
        #endregion


        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ZipEntry"/> struct.
        /// </summary>
        /// <param name="shellItem">The shell item.</param>
        public ZipEntry(FolderItem shellItem)
        {
            m_ShellItem = shellItem;
        }
        #endregion
    }

}
