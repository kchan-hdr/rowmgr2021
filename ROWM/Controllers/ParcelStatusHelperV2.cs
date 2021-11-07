using Microsoft.EntityFrameworkCore;
using ROWM.Dal;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;

namespace ROWM.Controllers
{
    public interface IParcelStatusHelper
    {
        string ParseDomainValue(int d);
        int GetDomainValue(string status);
        int GetRank(string status);
        int GetRoeDomainValue(string status);

        string ParseScore(int? score);
        bool IsValidScore(int? score);
    }

    /// <summary>
    /// collapsed Parcel_Status & Roe_Status
    /// </summary>
    public class ParcelStatusHelperV2 : IParcelStatusHelper
    {
        readonly List<ParcelStatus> _Status;
        readonly Dictionary<int, LandownerScore> _Scores;

        public ParcelStatusHelperV2(ROWM_Context c)
        {
            _Status = c.ParcelStatuses.AsNoTracking().ToList();
            _Scores = c.LandownerScores.AsNoTracking().ToDictionary<LandownerScore, int>(lls => lls.Score);
        }

        public string ParseDomainValue(int d)
        {
            var s = _Status.Single(sx => sx.DomainValue == d);
            return s.Code;
        }

        public int GetDomainValue(string status)
        {
            var s = _Status.Single(sx => sx.Code.Equals(status));
            return s.DomainValue;
        }

        public int GetRank(string status) => _Status.SingleOrDefault(sx => sx.Code.Equals(status))?.DisplayOrder ?? 0;

        public int GetRoeDomainValue(string status) => GetDomainValue(status);

        public static bool HasNoContact(Parcel parcel) => parcel.ParcelStatusCodeNavigation.DomainValue <= 0;

        #region landowner score 
        public string ParseScore(int? score)
        {
            if (!score.HasValue || score == 0) { return ""; }
            var val = score.Value;
            return _Scores.ContainsKey(val) ? _Scores[val].Caption : "";
        }
        public bool IsValidScore(int? score) => (!score.HasValue || score == 0) ? false : _Scores.ContainsKey(score.Value);
        #endregion
    }
}
