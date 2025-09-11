cmake_minimum_required(VERSION 3.15)
project(OpenGL)

set(CMAKE_CXX_STANDARD 17)

# --- Paths ---
set(DEPS_DIR ${CMAKE_SOURCE_DIR}/Dependencies)
set(INCLUDE_DIR ${DEPS_DIR}/include)

# --- Source files ---
file(GLOB_RECURSE PROJECT_SOURCES
    ${CMAKE_SOURCE_DIR}/OpenGL/*.cpp
    ${CMAKE_SOURCE_DIR}/OpenGL/Util/*.cpp
)

# Add glad.c (in root)
list(APPEND PROJECT_SOURCES ${CMAKE_SOURCE_DIR}/glad.c)

# --- Executable ---
add_executable(${PROJECT_NAME} ${PROJECT_SOURCES})

# --- Includes (vendor header-only deps) ---
target_include_directories(${PROJECT_NAME} PRIVATE
    ${INCLUDE_DIR}
    ${INCLUDE_DIR}/glad
    ${INCLUDE_DIR}/GLFW
    ${INCLUDE_DIR}/glm
    ${INCLUDE_DIR}/assimp
    ${INCLUDE_DIR}/KHR
)

# --- Dependency Management ---
include(FetchContent)

# GLFW
FetchContent_Declare(
  glfw
  GIT_REPOSITORY https://github.com/glfw/glfw.git
  GIT_TAG 3.3.8   # stable release
)
FetchContent_MakeAvailable(glfw)

# Assimp
FetchContent_Declare(
  assimp
  GIT_REPOSITORY https://github.com/assimp/assimp.git
  GIT_TAG v5.4.3  # stable release
)
set(ASSIMP_BUILD_TESTS OFF CACHE BOOL "" FORCE)
set(ASSIMP_BUILD_ASSIMP_TOOLS OFF CACHE BOOL "" FORCE)
set(ASSIMP_BUILD_ALL_EXPORTERS_BY_DEFAULT FALSE CACHE BOOL "" FORCE)
set(ASSIMP_BUILD_ALL_IMPORTERS_BY_DEFAULT TRUE CACHE BOOL "" FORCE)
FetchContent_MakeAvailable(assimp)

# --- Link libraries ---
if (WIN32)
    target_link_libraries(${PROJECT_NAME} PRIVATE glfw opengl32 assimp)
elseif (APPLE)
    find_library(COCOA_LIBRARY Cocoa REQUIRED)
    find_library(IOKIT_LIBRARY IOKit REQUIRED)
    find_library(COREVIDEO_LIBRARY CoreVideo REQUIRED)

    target_link_libraries(${PROJECT_NAME}
        PRIVATE
        glfw
        assimp
        ${COCOA_LIBRARY}
        ${IOKIT_LIBRARY}
        ${COREVIDEO_LIBRARY}
    )
else() # Linux
    target_link_libraries(${PROJECT_NAME}
        PRIVATE
        glfw
        assimp
        GL
        X11
        pthread
        Xrandr
        Xi
        dl
    )
endif()

# --- NEW SECTION: Copy VSCode Config Based on Platform ---
set(VSCODE_ROOT "${CMAKE_SOURCE_DIR}/.vscode")

if(WIN32)
    set(VSCODE_SOURCE "${CMAKE_SOURCE_DIR}/vscodeConfig/win/.vscode")
elseif(APPLE)
    set(VSCODE_SOURCE "${CMAKE_SOURCE_DIR}/vscodeConfig/mac/.vscode")
else()
    message(WARNING "Unknown platform, not copying VSCode config.")
endif()

if(EXISTS "${VSCODE_SOURCE}")
    file(REMOVE_RECURSE "${VSCODE_ROOT}")  # Remove old .vscode if exists
    file(COPY "${VSCODE_SOURCE}" DESTINATION "${CMAKE_SOURCE_DIR}")
    message(STATUS "Copied VSCode config from ${VSCODE_SOURCE} to ${VSCODE_ROOT}")
else()
    message(WARNING "VSCode config source not found: ${VSCODE_SOURCE}")
endif()