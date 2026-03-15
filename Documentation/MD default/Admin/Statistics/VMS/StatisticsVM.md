# Statistics ViewModels

## File Structure
- **StatisticsVM**: StatisticsVM.cs
- **AgeGroupStatVM**: AgeGroupStatVM.cs  
- **CourseCategoryStatVM**: CourseCategoryStatVM.cs
- **GenderStatVM**: GenderStatVM.cs
- **ResidentsAndCitizensStatVM**: ResidentsAndCitizensStatVM.cs

## Core ViewModels

### StatisticsVM
**Purpose**: Main container ViewModel for comprehensive statistical data and analytics

**Properties**:
- `TotalInActivities`: Total number of participants in activities (int)
- `TotalInCourses`: Total number of participants in courses (int)
- `AgeGroupStats`: Collection of age group statistics (List<AgeGroupStatVM>)
- `CourseDepartmentCategoryStats`: Collection of course department statistics (List<CourseCategoryStatVM>)
- `GenderStats`: Collection of gender distribution statistics (List<GenderStatVM>)
- `ResidentsAndCitizensStats`: Collection of residency status statistics (List<ResidentsAndCitizensStatVM>)

### AgeGroupStatVM
**Purpose**: ViewModel for age group demographic statistics

**Properties**:
- `Label`: Age group category label (string)
- `Percentage`: Participation percentage for the age group (float)
- `Count`: Number of participants in the age group (int)

### CourseCategoryStatVM
**Purpose**: ViewModel for course participation statistics by department/category

**Properties**:
- `Label`: Department or course category label (string)
- `Percentage`: Participation percentage for the category (float)
- `Count`: Number of participants in the category (int)

### GenderStatVM
**Purpose**: ViewModel for gender distribution statistics

**Properties**:
- `Label`: Gender category label (string)
- `Percentage`: Participation percentage for the gender (float)
- `Count`: Number of participants for the gender (int)

### ResidentsAndCitizensStatVM
**Purpose**: ViewModel for residency status statistics (Citizens vs Residents)

**Properties**:
- `Label`: Residency status label (string)
- `Percentage`: Participation percentage for the residency status (float)
- `Count`: Number of participants with the residency status (int)

## Key Features

### Comprehensive Analytics
- **Multi-dimensional Data**: Covers activities, courses, demographics, and departments
- **Percentage Calculations**: Normalized participation rates across categories
- **Absolute Counts**: Raw participant numbers for precise analysis
- **Categorical Breakdown**: Detailed segmentation of participant data

### Statistical Structure
- **Unified Container**: StatisticsVM aggregates all statistical categories
- **Consistent Pattern**: All stat VMs follow same Label/Percentage/Count pattern
- **Flexible Collections**: List-based structure supports dynamic category counts
- **Scalable Design**: Easy to add new statistical categories

### Data Presentation
- **Label-based Identification**: Clear category naming for UI display
- **Percentage Formatting**: Ready for visual representation in charts/graphs
- **Count Context**: Provides absolute numbers alongside percentages
- **Multi-category Support**: Handles various demographic and program dimensions

## Use Cases

### Dashboard Analytics
- Activity and course participation overview
- Demographic distribution analysis
- Department performance metrics
- Residency status participation patterns

### Reporting & Visualization
- Chart and graph data preparation
- Statistical report generation
- Comparative analysis between categories
- Trend identification across demographic segments

### Strategic Planning
- Resource allocation based on participation patterns
- Program development targeting specific demographics
- Department performance evaluation
- Membership engagement analysis

## Integration Benefits

### Frontend Compatibility
- **Clean Data Structure**: Optimized for UI component binding
- **Chart Library Ready**: Perfect for pie charts, bar graphs, and metrics displays
- **Responsive Design**: Supports various screen sizes and layouts
- **Localization Ready**: Label-based system for multi-language support

### Business Intelligence
- **Performance Metrics**: Track program effectiveness
- **Demographic Insights**: Understand participant composition
- **Growth Tracking**: Monitor participation trends over time
- **Decision Support**: Data-driven planning and optimization

### Technical Advantages
- **Type Safety**: Strongly typed statistical data
- **Serialization Friendly**: Clean structure for JSON/API responses
- **Validation Ready**: Easy to add data validation as needed
- **Extensible Design**: Simple to add new statistical dimensions