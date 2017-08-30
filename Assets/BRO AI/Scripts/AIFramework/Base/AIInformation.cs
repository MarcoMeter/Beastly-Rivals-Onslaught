namespace BRO.AI
{
    /// <summary>
    /// This class holds all the "overall" information about the AI
    /// </summary>
    [System.Serializable]
    public class AIInformation
    {
        private string _name;
        private string _author;
        private string _affinity;
        private string _description;
        private AITag _tag;
        private string _version;
        private string _creationDate;
        private string _pictureFileName;


        public AIInformation(
            string name,
            string author,
            string affinity,
            string description,
            AITag tag,
            string version,
            string creationDate,
            string pictureFileName)
        {
            this._name = name;
            this._author = author;
            this._affinity = affinity;
            this._description = description;
            this._tag = tag;
            this._version = version;
            this._creationDate = creationDate;
            this._pictureFileName = pictureFileName;
        }

        #region Getter
        public string GetName()
        {
            return this._name;
        }

        public string GetAuthor()
        {
            return this._author;
        }

        public string GetAffinity()
        {
            return this._affinity;
        }

        public string GetDescription()
        {
            return this._description;
        }

        public AITag GetTag()
        {
            return this._tag;
        }

        public string GetVersion()
        {
            return this._version;
        }

        public string GetDate()
        {
            return this._creationDate;
        }

        public string GetPictureFileName()
        {
            return this._pictureFileName;
        }
        #endregion
    }

    public enum AITag
    {
        Experimental,
        Competitive,
        LearningAI,
        Research,
        OfflineLearning,
        Teaching,
        Tutorial
    }
}
