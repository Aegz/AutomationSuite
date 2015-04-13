using AutomationSuiteFrontEnd.Utilities.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace AutomationSuiteFrontEnd.Utilities
{
    public class HTMLFactory
    {

        private static List<String> asIgnoreProperties = new List<String>
        {
            {"ExtensionData"}
        };

        private static List<String> asIgnoreFields = new List<String>
        {

        };

        private static Dictionary<Type, List<MemberInfo>> KnownTypes
        {
            get
            {
                // Get the Application State (Global Storage)
                HttpApplicationState oTemp = HttpContext.Current.Application;

                // Double lock check for Singleton behaviour
                if (oTemp["CachedTypes"] == null)
                {
                    // Lock
                    oTemp.Lock();
                    // Check again
                    if (oTemp["CachedTypes"] == null)
                    {
                        //Assign a new one
                        oTemp["CachedTypes"] = new Dictionary<Type, List<MemberInfo>>();
                    }
                    // Unlock
                    oTemp.UnLock();
                }

                // return the dictionary
                return (Dictionary<Type, List<MemberInfo>>)oTemp["CachedTypes"];
            }
        }

        #region Generation Specific
        private static List<MemberInfo> GetMemberList(Type xtType)
        {
            if (!KnownTypes.ContainsKey(xtType))
            {
                GenerateAndCacheTypeAttributes(xtType);
            }

            return KnownTypes[xtType];
        }

        private static void GenerateAndCacheTypeAttributes(Type xtGiven)
        {
            // Temporary return var
            List<MemberInfo> okpList = new List<MemberInfo>();

            // Get all the fields for the type
            System.Reflection.FieldInfo[] fieldInfo = xtGiven.GetFields();

            // For each field
            foreach (System.Reflection.FieldInfo info in fieldInfo)
            {
                if (!asIgnoreFields.Contains(info.Name))
                {
                    // Add to the return list/cache
                    okpList.Add(xtGiven.GetField(info.Name));
                }
            }

            // Get all the fields for the type
            System.Reflection.PropertyInfo[] propInfo = xtGiven.GetProperties();

            // For each field
            foreach (System.Reflection.PropertyInfo info in propInfo)
            {
                if (!asIgnoreProperties.Contains(info.Name))
                {
                    // Add to the return list/cache
                    okpList.Add(xtGiven.GetProperty(info.Name));
                }
            }

            // Add to the cache
            KnownTypes.Add(xtGiven, okpList);
        }
        #endregion



        #region Core Get/Set Properties
        public static void SetProperties(object xoGiven, List<KeyValuePair<String, String>> xasKeyValuePairs)
        {
            // Get the type of the object we need to assign to
            Type xtType = xoGiven.GetType();

            // Get a list and sort it alphabetically
            List<MemberInfo> okpTemp = GetMemberList(xtType);
            okpTemp.Sort(
                (a, b) =>
                {
                    return a.Name.CompareTo(b.Name);
                }
                );

            // Smallest list on the outside (more efficient)
            if (okpTemp.Count > xasKeyValuePairs.Count)
            {
                foreach (KeyValuePair<String, String> kvpLoopingVar in xasKeyValuePairs)
                {
                    // Try and retrieve a matching Info
                    MemberInfo oMatchingInfo = okpTemp.Find((oItem) => oItem.Name == kvpLoopingVar.Key);

                    // If any object matches
                    if (oMatchingInfo.Name != null)
                    {
                        SetSingleProperty(oMatchingInfo, xoGiven, kvpLoopingVar.Value);
                    }
                }
            }
            else
            {
                // For each property
                foreach (MemberInfo oLoopingVar in okpTemp)
                {
                    // Try and retrieve a matching KVP
                    KeyValuePair<String, String> oMatchingKVP = xasKeyValuePairs.Find((kvp) => kvp.Key == oLoopingVar.Name);

                    // If any object matches
                    if (oMatchingKVP.Key != null)
                    {
                        SetSingleProperty(oLoopingVar, xoGiven, oMatchingKVP.Value);
                    }
                }
            }
            
        }

        public static ExpandableRow[] GetProperties(object xoGiven)
        {
            Type tObjectType = xoGiven.GetType();
            // Retrieve and return as array
            return GetMemberList(tObjectType).Select(
                (oItem) => new ExpandableRow ( 
                    oItem.Name, 
                    GetSingleProperty(oItem, xoGiven), 
                    tObjectType)
                    ).ToArray();
        }


        #endregion

        #region Private Single Property/Field methods
        private static void SetSingleProperty(MemberInfo xoInfo, object xoGiven, String xsValue)
        {
            // 
            if (xoInfo.MemberType == MemberTypes.Property)
            {
                PropertyInfo pTemp = (PropertyInfo)xoInfo;
                pTemp.SetValue(xoGiven, xsValue);
   
            }
            else if (xoInfo.MemberType == MemberTypes.Field)
            {
                FieldInfo fTemp = (FieldInfo)xoInfo;
                fTemp.SetValue(xoGiven, xsValue);
            }
            else
            {
                // Invalid?
                throw new Exception("Tried to set an invalid property: " + xoInfo.Name + " & Type: " + xoGiven.GetType().ToString());
            }
        }

        private static String GetSingleProperty(MemberInfo xoInfo, object xoGiven)
        {
            // 
            if (xoInfo.MemberType == MemberTypes.Property)
            {
                PropertyInfo pTemp = (PropertyInfo)xoInfo;
                return Convert.ToString(pTemp.GetValue(xoGiven));

            }
            else if (xoInfo.MemberType == MemberTypes.Field)
            {
                FieldInfo fTemp = (FieldInfo)xoInfo;
                return Convert.ToString(fTemp.GetValue(xoGiven));
            }
            else
            {
                // Invalid?
                throw new Exception("Tried to GET an invalid property: " + xoInfo.Name + " & Type: " + xoGiven.GetType().ToString());
            }
        }
        #endregion
    }

}