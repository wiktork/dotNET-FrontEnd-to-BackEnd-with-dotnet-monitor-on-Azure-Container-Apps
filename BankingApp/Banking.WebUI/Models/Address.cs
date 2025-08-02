using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Banking.WebUI.Models
{
    [DataContract]
    public class Address
    {
        [DataMember]
        public string Street { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string State { get; set; }
        [DataMember]
        public string ZipCode { get; set; }

        public Address()
        {

        }

        public Address(string street, string city, string state, string zipCode)
        {
            Street = street;
            City = city;
            State = state;
            ZipCode = zipCode;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Address otherAddress = (Address)obj;

            bool isEqual = Street == otherAddress.City && City == otherAddress.City &&
                       State == otherAddress.State && ZipCode == otherAddress.ZipCode;

            return isEqual;
        }

        public string ToJson()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Address));
                serializer.WriteObject(stream, this);
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8, true, 1024, true))
                {
                    string json = reader.ReadToEnd();
                    return json;
                }
            }
        }

        public static Address FromJson(string json)
        {
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Address));
                return (Address)serializer.ReadObject(stream);
            }
        }
    }
}
