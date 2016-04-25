using System;
using System.Collections.Generic;
using System.Linq;
using VirtualNote.Kernel;
using VirtualNote.Kernel.DTO;

namespace VirtualNote.MVC.Enums
{
    public static class TypeEnumResolver
    {
        static readonly IDictionary<TypeEnum, String> Container = new Dictionary<TypeEnum, String>();

        static TypeEnumResolver() {
            Container.Add(TypeEnum.Bug, "Bug");
            Container.Add(TypeEnum.Info, "Information");
            Container.Add(TypeEnum.NewFeautures, "New features");
        }

        public static String GetByKey(TypeEnum key)
        {
            return Container[key];
        }

        public static IEnumerable<EnumClass> GetAll() {
            return Container.Keys.Select(k => new EnumClass
                                                  {
                                                      ID = (int) k, Text = Container[k]
                                                  }).ToList();
        }
    }

    public static class PriorityEnumResolver
    {
        static readonly IDictionary<PriorityEnum, String> Container = new Dictionary<PriorityEnum, String>();

        static PriorityEnumResolver() {
            Container.Add(PriorityEnum.Lowest, "Lowest");
            Container.Add(PriorityEnum.Low, "Low");
            Container.Add(PriorityEnum.Normal, "Normal");
            Container.Add(PriorityEnum.High, "High");
            Container.Add(PriorityEnum.Highest, "Highest");
        }
        public static String GetByKey(PriorityEnum key)
        {
            return Container[key];
        }

        public static IEnumerable<EnumClass> GetAll() {
            return Container.Keys.Select(k => new EnumClass
                                                  {
                                                      ID = (int) k, Text = Container[k]
                                                  }).ToList();
        }
    }

    public static class StateEnumResolver
    {
        static readonly IDictionary<StateEnum, String> Container = new Dictionary<StateEnum, String>();

        static StateEnumResolver() {
            Container.Add(StateEnum.Waiting, "Waiting");
            Container.Add(StateEnum.InResolution, "Resolution");
            Container.Add(StateEnum.Terminated, "Terminated");
        }
        public static String GetByKey(StateEnum key)
        {
            return Container[key];
        }

        public static IEnumerable<EnumClass> GetAll() {
            return Container.Keys.Select(k => new EnumClass
                                                  {
                                                      ID = (int) k, Text = Container[k]
                                                  }).ToList();
        }
    }

    public static class IssueSortByEnumResolver
    {
        static readonly IDictionary<IssuesSortBy, String> Container = new Dictionary<IssuesSortBy, String>();

        static IssueSortByEnumResolver() {
            Container.Add(IssuesSortBy.DescendingDate, "High Created Date");
            Container.Add(IssuesSortBy.DescendingPriority, "High Priority");
            Container.Add(IssuesSortBy.DescendingState, "State DESC");
            Container.Add(IssuesSortBy.DescendingType, "Type DESC");

            Container.Add(IssuesSortBy.AscendingDate, "Low Created Date");
            Container.Add(IssuesSortBy.AscendingPriority, "Low Priority");
            Container.Add(IssuesSortBy.AscendingState, "State ASC");
            Container.Add(IssuesSortBy.AscendingType, "Type ASC");
        }
        public static String GetByKey(IssuesSortBy key) {
            return Container[key];
        }

        public static IEnumerable<EnumClass> GetAll() {
            return Container.Keys.Select(k => new EnumClass {
                ID = (int)k,
                Text = Container[k]
            }).ToList();
        }
    }

}