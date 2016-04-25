using System;
using System.Collections.Generic;
using System.Linq;
using VirtualNote.Kernel.DTO.Query;

namespace VirtualNote.Kernel
{
    public enum TypeEnum {
        Bug = 0,
        Info = 1,
        NewFeautures = 2
    }

    public enum PriorityEnum {
        Lowest = 0,
        Low = 1,
        Normal = 2,
        High = 3,
        Highest = 4
    }

    public enum StateEnum {
        Waiting = 0,
        InResolution = 1,
        Terminated = 2
    }  
  
    public enum IssuesSortBy
    {
        DescendingDate = 0,
        AscendingDate = 1,

        DescendingState = 2,
        AscendingState = 3,

        DescendingPriority = 4,
        AscendingPriority = 5,

        DescendingType = 6,
        AscendingType = 7
    }


    public static class EnumUtils
    {
        public static IEnumerable<TypeEnum> GetTypeValues() {
            return Enum.GetValues(typeof(TypeEnum)).Cast<TypeEnum>().ToList();
        }

        public static IEnumerable<PriorityEnum> GetPriorityValues() {
            return Enum.GetValues(typeof(PriorityEnum)).Cast<PriorityEnum>().ToList();
        }

        public static IEnumerable<StateEnum> GetStateValues() {
            return Enum.GetValues(typeof(StateEnum)).Cast<StateEnum>().ToList();
        }

        public static IEnumerable<IssuesSortBy> GetIssuesSortValues() {
            return Enum.GetValues(typeof(IssuesSortBy)).Cast<IssuesSortBy>().ToList();
        }

        public static IEnumerable<KeyIdValueString> ToKivs(this IEnumerable<IssuesSortBy> collection) {
            return collection.Select(x => new KeyIdValueString {
                Id = (int)x,
                Value = x.ToString()
            }).ToList();
        }
    }
}
