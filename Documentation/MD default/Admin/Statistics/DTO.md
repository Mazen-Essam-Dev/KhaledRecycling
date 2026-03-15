# Statistics Data Transfer Objects

## File Structure
- **StatisticsDTO**: StatisticsDTO.cs
- **AgeGroupStat**: AgeGroupStat.cs  
- **CourseCategoryStat**: CourseCategoryStat.cs
- **GenderStat**: GenderStat.cs
- **ResidentsAndCitizensStat**: ResidentsAndCitizensStat.cs

## Data Transfer Objects

### StatisticsDTO
**Purpose**: Main DTO for transferring comprehensive statistical data between layers

**Properties**:
- `TotalInActivities`: Total participants in activities (int)
- `TotalInCourses`: Total participants in courses (int)
- `AgeGroupStats`: Age demographic statistics (List<AgeGroupStat>)
- `CourseDepartmentCategoryStats`: Department course statistics (List<CourseCategoryStat>)
- `GenderStats`: Gender distribution statistics (List<GenderStat>)
- `ResidentsAndCitizensStats`: Residency status statistics (List<ResidentsAndCitizensStat>)

### AgeGroupStat
**Purpose**: DTO for age group demographic data transfer

**Properties**:
- `Label`: Age category identifier (string)
- `Percentage`: Participation rate percentage (float)
- `Count`: Absolute participant count (int)

### CourseCategoryStat
**Purpose**: DTO for course participation data by department/category

**Properties**:
- `Label`: Department or course category name (string)
- `Percentage`: Category participation percentage (float)
- `Count`: Number of participants in category (int)

### GenderStat
**Purpose**: DTO for gender distribution data transfer

**Properties**:
- `Label`: Gender category identifier (string)
- `Percentage`: Gender participation percentage (float)
- `Count`: Participant count by gender (int)

### ResidentsAndCitizensStat
**Purpose**: DTO for residency status statistical data

**Properties**:
- `Label`: Residency status category (string)
- `Percentage`: Residency group participation percentage (float)
- `Count`: Participant count by residency status (int)

## Key Features

### Data Transfer Optimization
- **Lightweight Structure**: Minimal properties for efficient serialization
- **API Ready**: Perfect for REST API responses and client consumption
- **Serialization Friendly**: Clean structure for JSON/XML conversion
- **Performance Focused**: Optimized for network transfer

### Statistical Analysis
- **Multi-dimensional Metrics**: Comprehensive coverage of key statistics
- **Percentage-based Analysis**: Normalized participation rates
- **Absolute Counts**: Raw data for precise calculations
- **Categorical Segmentation**: Detailed breakdown across multiple dimensions

### Consistent Pattern
- **Uniform Structure**: All stat DTOs follow identical Label/Percentage/Count pattern
- **Scalable Collections**: List-based design supports dynamic data
- **Flexible Categorization**: Easy to extend with new statistical dimensions
- **Standardized Format**: Consistent data presentation across all categories

## Use Cases

### API Responses
- RESTful endpoint data delivery
- Mobile application statistics feeds
- External system integration
- Real-time dashboard updates

### Data Export
- External reporting systems
- Business intelligence tools
- Data analytics platforms
- Third-party integrations

### Internal Processing
- Service layer data transfer
- Repository pattern implementation
- Caching layer optimization
- Batch processing operations

## Integration Benefits

### Cross-layer Compatibility
- **Service Layer**: Efficient data retrieval and processing
- **Controller Layer**: Clean API response formatting
- **Client Applications**: Easy data consumption and visualization
- **External Systems**: Standardized data exchange format

### Performance Advantages
- **Minimal Payload**: Reduced network bandwidth usage
- **Fast Serialization**: Quick JSON/XML conversion
- **Efficient Storage**: Optimized for database and cache
- **Scalable Design**: Handles large datasets efficiently

### Development Efficiency
- **Clear Contract**: Well-defined data structure between layers
- **Type Safety**: Compile-time data validation
- **Easy Mapping**: Simple transformation from domain entities
- **Maintainable Code**: Consistent pattern across statistical data

## Business Intelligence Applications

### Analytics Reporting
- Participation trend analysis
- Demographic distribution reports
- Department performance metrics
- Program effectiveness evaluation

### Strategic Decision Support
- Resource allocation planning
- Target audience identification
- Program development guidance
- Membership growth strategies

### Performance Monitoring
- Real-time participation tracking
- Historical trend comparison
- Departmental performance assessment
- Demographic engagement analysis