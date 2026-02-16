using AutoMapper;
using TrainigSectorDataEntry.Models;
using TrainigSectorDataEntry.ViewModel;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<AlertsAndAdvertisment, AlertsAndAdvertismentVM>().ReverseMap();
        CreateMap<ComplaintsAndSuggestion, ComplaintsAndSuggestionVM>().ReverseMap();
        CreateMap<ContactU, ContactUVM>().ReverseMap();
        CreateMap<Departmentsandbranch, DepartmentsandbranchVM>().ReverseMap();
       // CreateMap<DepartmentsandBranchesDetail, DepartmentsandBranchesDetailVM>().ReverseMap();
        CreateMap<DepartmentsandBranchesImage, DepartmentsandBranchesImageVM>().ReverseMap();
        CreateMap<SpecializationImage, SpecializationImageVM>().ReverseMap();
        CreateMap<DepartmentType, DepartmentTypeVM>().ReverseMap();
        CreateMap<News, NewsVM>().ReverseMap();
        CreateMap<NewsImage, NewsImageVM>().ReverseMap();
        CreateMap<CommunityAndInternationalEngagement, CommunityAndInternationalEngagementVM>().ReverseMap();
        CreateMap<HistoryBreif, HistoryBreifVM>().ReverseMap();
        CreateMap<Project, ProjectVM>().ReverseMap();
        CreateMap<QualityCertificate, QualityCertificateVM>().ReverseMap();
        CreateMap<Slider, SliderVM>().ReverseMap();
        CreateMap<Specialization, SpecializationVM>().ReverseMap();
        CreateMap<StagesAndHall, StagesAndHallVM>().ReverseMap();
        CreateMap<StudentActivite, StudentActiviteVM>().ReverseMap();
        CreateMap<EducationalLevel, EducationalLevelVM>().ReverseMap();
        CreateMap<StudentsTimeTable, StudentsTimeTableVM>().ReverseMap();
        CreateMap<StudentTablesAttachment, StudentTablesAttachmentVM>().ReverseMap();
        CreateMap<SucessStory, SucessStoryVM>().ReverseMap();
        CreateMap<TrainingCoursesType, TrainingCoursesTypeVM>().ReverseMap();
        CreateMap<TrainingCourse, TrainingCourseVM>().ReverseMap();
    }
}
