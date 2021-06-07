using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using InterlockingDemo.Classes;
using InterlockingDemo.Helpler;
using InterlockingDemo.Views;
using System;
using System.Collections.Generic;
using System.Windows;

namespace InterlockingDemo.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region <properties>
        public Structure structure;

        private string copyright_name;
        public string Copyright_name
        {
            get { return copyright_name; }
            set { copyright_name = value;RaisePropertyChanged(); }
        }

        private string copyright_univer;
        public string Copyright_univer
        {
            get { return copyright_univer; }
            set { copyright_univer = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// �洢�����д���Ľ�·
        /// </summary>
        public List<Tuple<string, string>> Switch_with_errors;

        /// <summary>
        /// �洢�����д���Ľ�·
        /// </summary>
        public List<Tuple<string, string>> Section_with_errors;
        #endregion


        public MainViewModel()
        {
            InitProperties();
            InitCommands();
        }


        #region <commands>
        public RelayCommand SelectInterlockingFileCM { get; set; }
        public RelayCommand SelectSignalFileCM { get; set; }
        public RelayCommand SelectSwitchFileCM { get; set; }
        public RelayCommand SelectSectionFileCM { get; set; }
        public RelayCommand CheckInterlockingFeasibilityCM { get; set; }
        #endregion


        #region <functions>
        public void InitProperties()
        {
            structure = new Structure();
            Copyright_name = "���� by Pu Fan";
            Copyright_univer = "Beijing Jiaotong University";
            Section_with_errors = new List<Tuple<string, string>>();
            Switch_with_errors = new List<Tuple<string, string>>();
        }

        public void InitCommands()
        {
            SelectInterlockingFileCM = new RelayCommand(SelectInterlockingFileFunc);
            SelectSignalFileCM = new RelayCommand(SelectSignalFileFunc);
            SelectSwitchFileCM = new RelayCommand(SelectSwitchFileFunc);
            SelectSectionFileCM = new RelayCommand(SelectSectionFileFunc);
            CheckInterlockingFeasibilityCM = new RelayCommand(CheckInterlockingFeasibilityFunc);
        }

        public void SelectInterlockingFileFunc()
        {
            try
            {
                structure.Routes = EXCELHelper.ReadRouteEXCELFile();
            }
            catch
            {
                MessageBox.Show("��ʧ�ܣ��������ݣ�", "����", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SelectSignalFileFunc()
        {
            try
            {
                structure.Signals = EXCELHelper.ReadSignalEXCELFile();
            }
            catch
            {
                MessageBox.Show("��ʧ�ܣ��������ݣ�", "����", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SelectSwitchFileFunc()
        {
            try
            {
                structure.Switches = EXCELHelper.ReadSwitchEXCELFile();
            }
            catch
            {
                MessageBox.Show("��ʧ�ܣ��������ݣ�", "����", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SelectSectionFileFunc()
        {
            try
            {
                structure.Sections = EXCELHelper.ReadSectionEXCELFile();
            }
            catch
            {
                MessageBox.Show("��ʧ�ܣ��������ݣ�", "����", MessageBoxButton.OK, MessageBoxImage.Error);
            }            
        }

        /// <summary>
        /// ������������
        /// </summary>
        public void CheckInterlockingFeasibilityFunc()
        {            
            if (CheckDataIntegrility())
            {
                foreach(string route_key in structure.Routes.Keys)
                {
                    Route route = structure.Routes[route_key];
                    System.Type t = route.GetType();
                    //�ж����г���·���ǵ�����·
                    if (t == typeof(TrainRoute))
                    {
                        TrainRoute train_route = route as TrainRoute;
                        string direction = train_route.HeadToDirection;
                        string start_point = Route.FormateStringIntoEquipmentName(train_route.StartButton);

                        string end_point = train_route.EndButton;
                        if (end_point == "SLZA")
                        {
                            end_point = "D1";
                        }
                        else if (end_point == "XLZA")
                        {
                            end_point = "D2";
                        }
                        else if(end_point== "S��LA")
                        {
                            end_point = "D5";
                        }
                        else if(end_point== "X��LA")
                        {
                            end_point = "D10";
                        }
                        else
                        {
                            end_point = Route.FormateStringIntoEquipmentName(end_point);
                        }
                        List<List<string>> greedy_path = new List<List<string>>();
                        int index = 0;
                        Route.GenEquipmentString(structure, greedy_path, index, start_point, end_point, direction);
                        string switch_string = Route.GenSwitchString(structure, greedy_path);
                        train_route.GeneratedSwitchString = switch_string;
                        //��������ƥ��
                        if (switch_string != train_route.SwitchString)
                        {
                            Switch_with_errors.Add(new Tuple<string, string>(train_route.Name, switch_string));
                        }
                        string section_string = Route.GenSectionString(structure, greedy_path);
                        train_route.GeneratedSectionString = section_string;
                        //��������ƥ��
                        if (section_string != train_route.SectionString)
                        {
                            Section_with_errors.Add(new Tuple<string, string>(train_route.Name, section_string));
                        }
                    }
                    else if (t == typeof(ShuntRoute))
                    {
                        ShuntRoute shunt_route = route as ShuntRoute;
                        string direction = shunt_route.HeadToDirection;
                        string start_point = Route.FormateStringIntoEquipmentName(shunt_route.StartButton);
                        string end_point = shunt_route.EndButton;
                        if (end_point == "S��DA")
                        {
                            end_point = "D5";
                        }
                        else if (end_point == "X��DA")
                        {
                            end_point = "D10";
                        }
                        else
                        {
                            end_point = Route.FormateStringIntoEquipmentName(end_point);
                        }
                        List<List<string>> greedy_path = new List<List<string>>();
                        int index = 0;
                        Route.GenEquipmentString(structure, greedy_path, index, start_point, end_point, direction);
                        string switch_string = Route.GenSwitchString(structure, greedy_path);
                        shunt_route.GeneratedSwitchString = switch_string;
                        //��������ƥ��
                        if (switch_string != shunt_route.SwitchString)
                        {
                            Switch_with_errors.Add(new Tuple<string, string>(shunt_route.Name, switch_string));
                        }
                        string section_string = Route.GenSectionString(structure, greedy_path);
                        shunt_route.GeneratedSectionString = section_string;
                        //��������ƥ��
                        if (section_string != shunt_route.SectionString)
                        {
                            Section_with_errors.Add(new Tuple<string, string>(shunt_route.Name, section_string));
                        }
                    }
                }

                ShowErrorView view = new ShowErrorView(structure, Switch_with_errors, Section_with_errors);
                view.ShowDialog();
            }
            else
            {
                MessageBox.Show("�������ݲ�������", "����", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        /// <summary>
        /// �������������
        /// </summary>
        /// <returns></returns>
        private bool CheckDataIntegrility()
        {
            bool flag = true;
            if (structure.Routes.Count == 0 ||
                structure.Sections.Count == 0||
                structure.Signals.Count==0||
                structure.Switches.Count == 0)
            {
                flag = false;
            }
            return flag;
        }
        #endregion
    }
}