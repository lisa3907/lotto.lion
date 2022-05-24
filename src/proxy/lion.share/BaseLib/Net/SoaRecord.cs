﻿#region
//
// OdinSdk.BaseLib.Net.Dns by Rob Philpott, Big Developments Ltd. Please send all bugs/enhancements to
// rob@bigdevelopments.co.uk  This file and the code contained within is freeware and may be
// distributed and edited without restriction.
//

#endregion

#pragma warning disable 8632

namespace OdinSdk.BaseLib.Net.Dns
{
    /// <summary>
    /// An SOA Resource Record (RR) (RFC1035 3.3.13)
    /// </summary>
    public class SoaRecord : RecordBase
    {
        // these fields constitute an SOA RR
        private readonly string _primaryNameServer;

        private readonly string _responsibleMailAddress;
        private readonly int _serial;
        private readonly int _refresh;
        private readonly int _retry;
        private readonly int _expire;
        private readonly int _defaultTtl;

        // expose these fields public read/only
        public string PrimaryNameServer
        {
            get
            {
                return _primaryNameServer;
            }
        }

        public string ResponsibleMailAddress
        {
            get
            {
                return _responsibleMailAddress;
            }
        }

        public int Serial
        {
            get
            {
                return _serial;
            }
        }

        public int Refresh
        {
            get
            {
                return _refresh;
            }
        }

        public int Retry
        {
            get
            {
                return _retry;
            }
        }

        public int Expire
        {
            get
            {
                return _expire;
            }
        }

        public int DefaultTtl
        {
            get
            {
                return _defaultTtl;
            }
        }

        /// <summary>
        /// Constructs an SOA record by reading bytes from a return message
        /// </summary>
        /// <param name="pointer">A logical pointer to the bytes holding the record</param>
        internal SoaRecord(Pointer pointer)
        {
            // read all fields RFC1035 3.3.13
            _primaryNameServer = pointer.ReadDomain();
            _responsibleMailAddress = pointer.ReadDomain();
            _serial = pointer.ReadInt();
            _refresh = pointer.ReadInt();
            _retry = pointer.ReadInt();
            _expire = pointer.ReadInt();
            _defaultTtl = pointer.ReadInt();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
		public override string? ToString()
        {
            return String.Format("primary name server = {0}\nresponsible mail addr = {1}\nserial  = {2}\nrefresh = {3}\nretry   = {4}\nexpire  = {5}\ndefault TTL = {6}",
                _primaryNameServer,
                _responsibleMailAddress,
                _serial.ToString(),
                _refresh.ToString(),
                _retry.ToString(),
                _expire.ToString(),
                _defaultTtl.ToString());
        }
    }
}